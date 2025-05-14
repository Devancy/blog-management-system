using System.Threading.Tasks;

namespace BlogManagementSystem.Application.Interfaces;

public interface IIdentityManagerFactory
{
    /// <summary>
    /// Gets the current identity manager based on configuration.
    /// </summary>
    IIdentityManager CurrentManager { get; }
    
    /// <summary>
    /// Initializes the identity manager with the specified mode.
    /// </summary>
    /// <param name="mode">The identity mode to use ("keycloak" or "proxy").</param>
    Task Initialize(string mode);
    
    /// <summary>
    /// Gets the identity manager for the specified mode.
    /// </summary>
    /// <param name="mode">The identity mode to use ("keycloak" or "proxy").</param>
    IIdentityManager GetManager(string mode);
} 