using Havit.Data.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MB.HBlazorApp.Services.HealthChecks;

public class HBlazorAppDbContextHealthCheck : BaseHealthCheck
{
	private readonly IDbContext _dbContext;

	public HBlazorAppDbContextHealthCheck(IDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	protected async override Task<HealthCheckResult> CheckHealthAsync(CancellationToken cancellationToken)
	{
		return await _dbContext.Database.CanConnectAsync(cancellationToken)
			? HealthCheckResult.Healthy()
			: HealthCheckResult.Unhealthy();
	}
}
