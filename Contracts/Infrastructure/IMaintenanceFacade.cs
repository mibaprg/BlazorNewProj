using Havit.ComponentModel;

namespace MB.HBlazorApp.Contracts.Infrastructure;

[ApiContract]
public interface IMaintenanceFacade
{
	Task ClearCacheAsync(CancellationToken cancellationToken = default);
}
