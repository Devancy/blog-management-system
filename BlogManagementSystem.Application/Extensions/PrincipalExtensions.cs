using System.Security.Claims;

namespace BlogManagementSystem.Application.Extensions;

public static class PrincipalExtensions
{
	public static string? FindFirstValue(this ClaimsPrincipal principal, string claimType)
	{
		ArgumentNullException.ThrowIfNull(principal);
		var claim = principal.FindFirst(claimType);
		return claim?.Value;
	}
}