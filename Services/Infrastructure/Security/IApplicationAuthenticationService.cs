using System.Security.Claims;
using MB.HBlazorApp.Model.Security;

namespace MB.HBlazorApp.Services.Infrastructure.Security;

public interface IApplicationAuthenticationService
{
	ClaimsPrincipal GetCurrentClaimsPrincipal();
	User GetCurrentUser();
}
