using MB.HBlazorApp.Model.Security;

namespace MB.HBlazorApp.DataLayer.Repositories.Security;

public partial interface IUserRepository
{
	Task<User> GetByIdentityProviderIdAsync(string identityProviderId, CancellationToken cancellationToken = default);
}
