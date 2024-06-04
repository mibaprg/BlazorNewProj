using BlazorApplicationInsights;
using Hangfire;
using Hangfire.Dashboard;
using Havit.Blazor.Grpc.Server;
using MB.HBlazorApp.Contracts;
using MB.HBlazorApp.Contracts.Infrastructure;
using MB.HBlazorApp.DependencyInjection;
using MB.HBlazorApp.Facades.Infrastructure.Security;
using MB.HBlazorApp.Primitives.Security;
using MB.HBlazorApp.Services.HealthChecks;
using MB.HBlazorApp.Services.Infrastructure.Security;
using MB.HBlazorApp.Web.Client.Infrastructure.Configuration;
using MB.HBlazorApp.Web.Server.Infrastructure.ApplicationInsights;
using MB.HBlazorApp.Web.Server.Infrastructure.ConfigurationExtensions;
using MB.HBlazorApp.Web.Server.Infrastructure.ExceptionHandling;
using MB.HBlazorApp.Web.Server.Infrastructure.HealthChecks;
using MB.HBlazorApp.Web.Server.Infrastructure.MigrationTool;
using MB.HBlazorApp.Web.Server.Infrastructure.Security;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Components.Authorization;
using ProtoBuf.Grpc.Server;

namespace MB.HBlazorApp.Web.Server;

public class Startup
{
	private readonly IConfiguration _configuration;

	public Startup(IConfiguration configuration)
	{
		_configuration = configuration;
	}

	public void ConfigureServices(IServiceCollection services)
	{
		services.AddOptions();
		services.Configure<WebClientOptions>(_configuration.GetSection("WebClient"));

		services.ConfigureForWebServer(_configuration);

		services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

		services.AddDatabaseDeveloperPageExceptionFilter();

		services.AddCustomizedMailing(_configuration);

		// SmtpExceptionMonitoring to errors@havit.cz
		services.AddExceptionMonitoring(_configuration);

		// Application Insights
		services.AddApplicationInsightsTelemetry(_configuration);
		services.AddSingleton<ITelemetryInitializer, GrpcRequestStatusTelemetryInitializer>();
		services.AddSingleton<ITelemetryInitializer, CloudRoleNameTelemetryInitializer>();
		services.ConfigureTelemetryModule<DependencyTrackingTelemetryModule>((module, o) => { module.EnableSqlCommandTextInstrumentation = true; });

		// BlazorApplicationInsights
		// For prerendering BlazorApplicationInsights, we need to have the ApplicationInsightsInitConfig configuration registered in the DI container,
		// even though it is not actually used - OnAfterPrerender is not called on it.
		// The actual configuration is performed by the client.
		services.AddBlazorApplicationInsights(c => c.ConnectionString = "");

		// Authentication & Authorization
		services.AddCustomAuthentication(_configuration);
		services.AddAuthorization(options =>
		{
			options.AddPolicy(PolicyNames.HangfireDashboardAccessPolicy, policy => policy
					.AddAuthenticationSchemes(AuthenticationConfigurationExtension.MsOidcScheme)
					.RequireAuthenticatedUser()
					.RequireRole(nameof(RoleEntry.SystemAdministrator)));
		});

		services.AddScoped<AuthenticationStateProvider, PersistingAuthenticationStateProvider>();
		services.AddScoped<IApplicationAuthenticationService, ApplicationAuthenticationService>();

		// Blazor components
		services.AddRazorComponents()
			.AddInteractiveWebAssemblyComponents();

		// server-side UI
		services.AddRazorPages();
		services.AddControllersWithViews();

		// gRPC
		services.AddGrpcServerInfrastructure(assemblyToScanForDataContracts: typeof(Dto).Assembly);
		services.AddCodeFirstGrpcReflection();

		// Health checks
		TimeSpan defaultHealthCheckTimeout = TimeSpan.FromSeconds(10);
		services.AddHealthChecks()
			.AddCheck<HBlazorAppDbContextHealthCheck>("Database", timeout: defaultHealthCheckTimeout)
			.AddCheck<MailServiceHealthCheck>("SMTP", timeout: defaultHealthCheckTimeout);

		// Hangfire
		services.AddCustomizedHangfire(_configuration);

		// Migrations
		services.Configure<MigrationsOptions>(_configuration.GetSection(MigrationsOptions.Path));
		services.AddHostedService<MigrationHostedService>();
	}

	// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
	public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
	{
		if (env.IsDevelopment())
		{
			app.UseDeveloperExceptionPage();
			app.UseMigrationsEndPoint();
			app.UseWebAssemblyDebugging();
		}
		else
		{
			app.UseExceptionHandler("/error");
			// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
			// TODO app.UseHsts();
		}

		app.UseMiddleware<CustomResponseForKnownExceptionsMiddleware>();

		app.UseHttpsRedirection();
		app.UseStaticFiles();

		app.UseExceptionMonitoring();

		app.UseRouting();

		app.UseAuthentication();
		app.UseAuthorization();

		app.UseAntiforgery();

		app.UseGrpcWeb(new GrpcWebOptions() { DefaultEnabled = true });

		app.UseEndpoints(endpoints =>
		{
			endpoints.MapRazorPages();
			endpoints.MapControllers();
			endpoints.MapRazorComponents<App>()
				.AddInteractiveWebAssemblyRenderMode()
				.AddAdditionalAssemblies(typeof(MB.HBlazorApp.Web.Client.Program).Assembly);

			endpoints.MapGrpcServicesByApiContractAttributes(
				typeof(IDataSeedFacade).Assembly,
				configureEndpointWithAuthorization: endpoint =>
				{
					endpoint.RequireAuthorization(); // TODO? AuthorizationPolicyNames.ApiScopePolicy when needed
				});
			endpoints.MapCodeFirstGrpcReflectionService();

			endpoints.MapGroup("/authentication").MapLoginAndLogout();

			endpoints.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
			{
				AllowCachingResponses = false,
				ResponseWriter = HealthCheckWriter.WriteResponseAsync
			});

			endpoints.MapHangfireDashboard("/hangfire", new DashboardOptions
			{
				DefaultRecordsPerPage = 50,
				Authorization = new List<IDashboardAuthorizationFilter>(), // see https://sahansera.dev/securing-hangfire-dashboard-with-endpoint-routing-auth-policy-aspnetcore/
				DisplayStorageConnectionString = false,
				DashboardTitle = "HBlazorApp - Jobs",
				StatsPollingInterval = 60_000, // once a minute
				DisplayNameFunc = (_, job) => Havit.Hangfire.Extensions.Helpers.JobNameHelper.TryGetSimpleName(job, out string simpleName)
													? simpleName
													: job.ToString()
			})
			.RequireAuthorization(PolicyNames.HangfireDashboardAccessPolicy);
		});
	}
}
