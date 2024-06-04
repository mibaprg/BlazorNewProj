using System.Security.Claims;
using MB.HBlazorApp.Contracts.Infrastructure.Security;
using MB.HBlazorApp.DataLayer.Repositories.Security;
using MB.HBlazorApp.Model.Security;
using MB.HBlazorApp.Services.Infrastructure.Security;

namespace MB.HBlazorApp.Web.Server.Infrastructure.Security;

public class ApplicationAuthenticationService : IApplicationAuthenticationService
{
	private readonly IHttpContextAccessor _httpContextAccessor;

	private readonly Lazy<User> _userLazy;

	public ApplicationAuthenticationService(IHttpContextAccessor httpContextAccessor, IUserRepository userRepository)
	{
		_httpContextAccessor = httpContextAccessor;

		_userLazy = new Lazy<User>(() => userRepository.GetObject(GetCurrentUserId()));
	}

	public ClaimsPrincipal GetCurrentClaimsPrincipal()
	{
		return _httpContextAccessor.HttpContext.User;
	}

	public User GetCurrentUser() => _userLazy.Value;

	public int GetCurrentUserId()
	{
		var principal = GetCurrentClaimsPrincipal();
		Claim userIdClaim = principal.Claims.Single(claim => (claim.Type == ClaimConstants.UserIdClaim));
		return Int32.Parse(userIdClaim.Value);
	}
}
