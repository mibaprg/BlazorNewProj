using MB.HBlazorApp.Primitives.Security;

namespace MB.HBlazorApp.Services.Infrastructure.Security;

public interface IApplicationAuthorizationService
{
	IEnumerable<RoleEntry> GetCurrentUserRoles();

	bool IsCurrentUserInRole(RoleEntry role);
}
