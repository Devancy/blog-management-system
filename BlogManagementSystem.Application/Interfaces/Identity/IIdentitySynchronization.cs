using System.Threading;
using System.Threading.Tasks;

namespace BlogManagementSystem.Application.Interfaces.Identity;

/// <summary>
/// Interface for identity synchronization operations
/// </summary>
public interface IIdentitySynchronization
{
    /// <summary>
    /// Synchronizes users from external identity providers
    /// </summary>
    Task<bool> SynchronizeUsersAsync(CancellationToken cancellationToken = default);
}