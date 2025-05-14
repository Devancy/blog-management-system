using System.Security.Claims;
using BlogManagementSystem.Application.Common.Configuration;
using BlogManagementSystem.Application.Extensions;
using BlogManagementSystem.Application.Interfaces;
using BlogManagementSystem.Domain.Entities;

namespace BlogManagementSystem.Application.Services.Identity;

public class IdentityMappingService(
    ILocalUserIdentityRepository userIdentityRepository,
    ILocalRoleRepository roleRepository,
    ILocalGroupRepository groupRepository,
    IdentityConfig identityConfig)
    : IIdentityMappingService
{
    public async Task<ClaimsPrincipal> ProcessUserClaimsAsync(ClaimsPrincipal principal, CancellationToken cancellationToken = default)
    {
        // If not in proxy mode, return the original principal
        if (!identityConfig.UseKeycloakAsIdpProxy)
        {
            return principal;
        }
        
        try
        {
            // Extract user data from claims
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier) ?? 
                         principal.FindFirstValue("sub");
                             
            if (userId == null)
            {
                Console.WriteLine("User identifier claim not found in token");
                return principal;
            }
            
            var userName = principal.FindFirstValue(ClaimTypes.Name) ?? 
                          principal.FindFirstValue("preferred_username") ?? 
                          userId;
            var email = principal.FindFirstValue(ClaimTypes.Email);
            var firstName = principal.FindFirstValue(ClaimTypes.GivenName);
            var lastName = principal.FindFirstValue(ClaimTypes.Surname);
            var organization = principal.FindFirstValue("organization");
            
            // Create or update user in local database
            await UpsertUserIdentityAsync(userId, userName, email, firstName, lastName, organization, cancellationToken);
            
            var roles = await GetUserRolesAsync(userId, cancellationToken);
            
            var identity = new ClaimsIdentity(principal.Identity);
            
            // Add role claims, hard-coded type "roles" for compatibility
            foreach (var role in roles)
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, role));
                identity.AddClaim(new Claim("roles", role));
            }
            
            // Create new principal with enhanced claims
            var newPrincipal = new ClaimsPrincipal(identity);
            return newPrincipal;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing user claims: {ex.Message}");
            return principal;
        }
    }
    
    private async Task<LocalUserIdentity> UpsertUserIdentityAsync(
        string userId, 
        string userName, 
        string? email, 
        string? firstName, 
        string? lastName,
        string? organization,
        CancellationToken cancellationToken)
    {
        var userIdentity = new LocalUserIdentity
        {
            UserId = userId,
            Username = userName,
            Email = email ?? string.Empty,
            FirstName = firstName ?? string.Empty,
            LastName = lastName ?? string.Empty,
            Organization = organization ?? string.Empty,
            LastLoginAt = DateTime.UtcNow
        };
        
        return await userIdentityRepository.UpsertAsync(userIdentity, cancellationToken);
    }
    
    private async Task<List<string>> GetUserRolesAsync(string userId, CancellationToken cancellationToken)
    {
        // Get direct roles
        var userRoles = await roleRepository.GetByUserIdAsync(userId, cancellationToken);
        var roles = userRoles.Select(r => r.Name).ToList();
        
        // Get roles from groups
        var userGroups = await groupRepository.GetByUserIdAsync(userId, cancellationToken);
        foreach (var group in userGroups)
        {
            var groupRoles = await roleRepository.GetByGroupPathAsync(group.Path, cancellationToken);
            roles.AddRange(groupRoles.Select(r => r.Name));
        }
        
        // Return unique roles
        return roles.Distinct().ToList();
    }
} 