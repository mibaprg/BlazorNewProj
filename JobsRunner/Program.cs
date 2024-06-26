﻿using Hangfire;
using Hangfire.Console;
using Hangfire.Console.Extensions;
using Hangfire.SqlServer;
using Hangfire.States;
using Havit.ApplicationInsights.DependencyCollector;
using Havit.AspNetCore.ExceptionMonitoring.Services;
using Havit.Hangfire.Extensions.BackgroundJobs;
using Havit.Hangfire.Extensions.Filters;
using Havit.Hangfire.Extensions.RecurringJobs;
using MB.HBlazorApp.DependencyInjection;
using MB.HBlazorApp.DependencyInjection.Configuration;
using MB.HBlazorApp.JobsRunner.Infrastructure.ApplicationInsights;
using MB.HBlazorApp.Services.Jobs;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Extensibility.PerfCounterCollector;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MB.HBlazorApp.JobsRunner;

public static class Program
{
	public static async Task Main(string[] args)
	{
		bool useHangfire = args.Length == 0;

		IHostBuilder hostBuilder = Host.CreateDefaultBuilder()
			.ConfigureAppConfiguration((hostContext, config) =>
			{
				config
					.AddJsonFile("appsettings.JobsRunner.json", optional: false)
					.AddJsonFile($"appsettings.JobsRunner.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true)
#if DEBUG
					.AddJsonFile($"appsettings.JobsRunner.{hostContext.HostingEnvironment.EnvironmentName}.local.json", optional: true) // .gitignored
#endif
					.AddEnvironmentVariables()
					.AddCustomizedAzureKeyVault();
			})
			.ConfigureLogging(logging =>
			{
				logging.AddSimpleConsole(configure => configure.TimestampFormat = "[HH:mm:ss] ");
			})
			.ConfigureServices((hostContext, services) =>
			{
				services.AddMemoryCache();

				services.AddApplicationInsightsTelemetryWorkerService();
				services.AddApplicationInsightsTelemetryProcessor<IgnoreSucceededDependenciesWithNoParentIdProcessor>();
				services.Remove(services.Single(descriptor => descriptor.ImplementationType == typeof(PerformanceCollectorModule)));
				services.AddSingleton<ITelemetryInitializer, JobsRunnerToCloudRoleNameTelemetryInitializer>();

				services.AddExceptionMonitoring(hostContext.Configuration);

				services.ConfigureForJobsRunner(hostContext.Configuration);

				if (useHangfire)
				{
					services.AddHangfire((serviceProvider, configuration) => configuration
						.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
						.UseSimpleAssemblyNameTypeSerializer()
						.UseRecommendedSerializerSettings()
						.UseSqlServerStorage(() => new Microsoft.Data.SqlClient.SqlConnection(hostContext.Configuration.GetConnectionString("Database")), new SqlServerStorageOptions
						{
							CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
							SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
							QueuePollInterval = TimeSpan.FromSeconds(5),
							UseRecommendedIsolationLevel = true,
							DisableGlobalLocks = true,
							EnableHeavyMigrations = true
						})
						.WithJobExpirationTimeout(TimeSpan.FromDays(30)) // history
						.UseFilter(new AutomaticRetryAttribute { Attempts = 0 }) // do not retry failed jobs
						.UseFilter(new ContinuationsSupportAttribute(new HashSet<string> { FailedState.StateName, DeletedState.StateName, SucceededState.StateName })) // only valid with AutomaticRetryAttribute with Attempts = 0
						.UseFilter(new CancelRecurringJobWhenAlreadyInQueueOrCurrentlyRunningFilter())
						.UseFilter(new ExceptionMonitoringAttribute(serviceProvider.GetRequiredService<IExceptionMonitoringService>()))
						.UseFilter(new ApplicationInsightAttribute(serviceProvider.GetRequiredService<TelemetryClient>()) { JobNameFunc = backgroundJob => Havit.Hangfire.Extensions.Helpers.JobNameHelper.TryGetSimpleName(backgroundJob.Job, out string simpleName) ? simpleName : backgroundJob.Job.ToString() })
						.UseConsole()
					);

					services.AddHangfireConsoleExtensions(); // adds support for Hangfire jobs logging  to a dashboard using ILogger<T> (.UseConsole() in hangfire configuration is required!)

#if DEBUG
					services.AddHangfireEnqueuedJobsCleanupOnApplicationStartup();
#endif
					services.AddHangfireRecurringJobsSchedulerOnApplicationStartup(GetRecurringJobsToSchedule().ToArray());

					// Add the processing server as IHostedService
					services.AddHangfireServer(o => o.WorkerCount = 1);
				}
			});

		IHost host = hostBuilder.Build();

		if (useHangfire)
		{
			// Run with Hangfire
			using (WebJobsShutdownWatcher webJobsShutdownWatcher = new WebJobsShutdownWatcher())
			{
				await host.RunAsync(webJobsShutdownWatcher.Token);
			}
		}
		else
		{
			// Run with command line
			if ((args.Length > 1) || (!await TryRunCommandAsync(host.Services, args[0])))
			{
				ShowCommandsHelp();
			}
		}
	}

	private static IEnumerable<IRecurringJob> GetRecurringJobsToSchedule()
	{
		TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");

		yield return new RecurringJob<IEmptyJob>(job => job.ExecuteAsync(CancellationToken.None), Cron.Minutely(), timeZone);
	}

	private static async Task<bool> TryRunCommandAsync(IServiceProvider serviceProvider, string command)
	{
		Contract.Requires<ArgumentNullException>(serviceProvider != null);
		Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(command));

		var job = GetRecurringJobsToSchedule().SingleOrDefault(job => String.Equals(job.JobId, command, StringComparison.CurrentCultureIgnoreCase));
		if (job == null)
		{
			return false;
		}

		using (var scopeService = serviceProvider.CreateScope())
		{
			IExceptionMonitoringService exceptionMonitoringService = serviceProvider.GetRequiredService<IExceptionMonitoringService>();
			try
			{
				await job.RunAsync(scopeService.ServiceProvider, CancellationToken.None);
			}
			catch (Exception ex)
			{
				exceptionMonitoringService.HandleException(ex);

				throw;
			}
		}

		return true;
	}

	private static void ShowCommandsHelp()
	{
		Console.WriteLine("Supported commands:");
		foreach (var job in GetRecurringJobsToSchedule().OrderBy(job => job.JobId).ToList())
		{
			Console.WriteLine("  " + job.JobId);
		}
	}
}
