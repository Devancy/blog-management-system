using System;using System.Collections.Generic;using System.Linq;using System.Security.Claims;using System.Threading;using System.Threading.Tasks;using BlogManagementSystem.Application.Common.Configuration;using BlogManagementSystem.Application.DTOs;using BlogManagementSystem.Application.Interfaces;using BlogManagementSystem.Domain.Entities;

namespace BlogManagementSystem.Application.Services.Identity;

public class IdentityMappingService : IIdentityMappingService
{
    private readonly ILocalUserIdentityRepository _userIdentityRepository;
    private readonly ILocalRoleRepository _roleRepository;
    private readonly ILocalGroupRepository _groupRepository;
    private readonly IdentityConfig _identityConfig;
    
    public IdentityMappingService(
        ILocalUserIdentityRepository userIdentityRepository,
        ILocalRoleRepository roleRepository,
        ILocalGroupRepository groupRepository,
        IdentityConfig identityConfig)
    {
        _userIdentityRepository = userIdentityRepository;
        _roleRepository = roleRepository;
        _groupRepository = groupRepository;
        _identityConfig = identityConfig;
    }
    
    public async Task<ClaimsPrincipal> ProcessUserClaimsAsync(ClaimsPrincipal principal, CancellationToken cancellationToken = default)
    {
        // If not in proxy mode, return the original principal
        if (!_identityConfig.UseKeycloakAsIdpProxy)
        {
            return principal;
        }
        
        try
        {
            // Extract user data from claims
            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier) ?? 
                             principal.FindFirst("sub");
                             
            if (userIdClaim == null)
            {
                Console.WriteLine("User identifier claim not found in token");
                return principal;
            }
            
            var userId = userIdClaim.Value;
            var userName = principal.FindFirst(ClaimTypes.Name)?.Value ?? 
                          principal.FindFirst("preferred_username")?.Value ?? 
                          userId;
            var email = principal.FindFirst(ClaimTypes.Email)?.Value;
            var firstName = principal.FindFirst(ClaimTypes.GivenName)?.Value;
            var lastName = principal.FindFirst(ClaimTypes.Surname)?.Value;
            var organization = principal.FindFirst("organization")?.Value;
            
            // Create or update user in local database
            var userIdentity = await UpsertUserIdentityAsync(userId, userName, email, firstName, lastName, organization, cancellationToken);
            
            // Get roles from the database for this user
            var roles = await GetUserRolesAsync(userId, cancellationToken);
            
            // Create a new claims identity with the additional roles
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
        
        return await _userIdentityRepository.UpsertAsync(userIdentity, cancellationToken);
    }
    
    private async Task<List<string>> GetUserRolesAsync(string userId, CancellationToken cancellationToken)
    {
        // Get direct roles
        var userRoles = await _roleRepository.GetByUserIdAsync(userId, cancellationToken);
        var roles = userRoles.Select(r => r.Name).ToList();
        
        // Get roles from groups
        var userGroups = await _groupRepository.GetByUserIdAsync(userId, cancellationToken);
        foreach (var group in userGroups)
        {
            // Get roles for this group
            var groupRoles = await _roleRepository.GetByGroupPathAsync(group.Path, cancellationToken);
            roles.AddRange(groupRoles.Select(r => r.Name));
        }
        
        // Return unique roles
        return roles.Distinct().ToList();
    }
} 