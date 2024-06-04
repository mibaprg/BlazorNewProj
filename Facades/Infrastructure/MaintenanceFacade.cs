using Havit.Extensions.DependencyInjection.Abstractions;
using MB.HBlazorApp.Contracts.Infrastructure;
using MB.HBlazorApp.Model.Security;
using MB.HBlazorApp.Primitives.Security;
using Havit.Services.Caching;
using Microsoft.AspNetCore.Authorization;

namespace MB.HBlazorApp.Facades.Infrastructure;

[Service]
[Authorize(Roles = nameof(RoleEntry.SystemAdministrator))]
public class MaintenanceFacade : IMaintenanceFacade
{
	private readonly ICacheService _cacheService;

	public MaintenanceFacade(ICacheService cacheService)
	{
		_cacheService = cacheService;
	}

	public Task ClearCacheAsync(CancellationToken cancellationToken = default)
	{
		_cacheService.Clear();

		return Task.CompletedTask;
	}
}
