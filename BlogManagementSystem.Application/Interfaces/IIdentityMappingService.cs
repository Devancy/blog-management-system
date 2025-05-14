using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace BlogManagementSystem.Application.Interfaces;

public interface IIdentityMappingService
{
    /// <summary>
    /// Processes the user claims from a ClaimsPrincipal and enhances it with locally stored roles
    /// and group memberships when in Keycloak proxy mode.
    /// </summary>
    /// <param name="principal">The original ClaimsPrincipal from authentication</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A new ClaimsPrincipal with enhanced claims, or the original if no enhancement is needed</returns>
    Task<ClaimsPrincipal> ProcessUserClaimsAsync(ClaimsPrincipal principal, CancellationToken cancellationToken = default);
} 