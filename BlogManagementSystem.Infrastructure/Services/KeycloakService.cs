using BlogManagementSystem.Application.Interfaces;
using BlogManagementSystem.Domain.Entities;
using Microsoft.Extensions.Configuration;
using System.Net;
using BlogManagementSystem.Application.DTOs;
using BlogManagementSystem.Infrastructure.Indentity.Mapping;
using BlogManagementSystem.Infrastructure.Indentity.Models;

namespace BlogManagementSystem.Infrastructure.Services;

public class KeycloakService(IKeycloakAdminClient keycloakClient, IConfiguration configuration)
    : IKeycloakService
{
    private readonly string _realm = configuration["Keycloak:realm"] ?? "master";
    private readonly string _adminUsername = configuration["Keycloak:AdminUsername"] ?? "admin";
    private readonly string _adminPassword = configuration["Keycloak:AdminPassword"] ?? "admin";
    private string? _accessToken;
    private DateTime _tokenExpiry = DateTime.MinValue;

    private async Task EnsureValidTokenAsync()
    {
        if (string.IsNullOrEmpty(_accessToken) || DateTime.UtcNow >= _tokenExpiry)
        {
            var tokenRequest = new Dictionary<string, string>
            {
                { "grant_type", "password" },
                { "client_id", "admin-cli" },
                { "username", _adminUsername },
                { "password", _adminPassword }
            };

            try
            {
                var response = await keycloakClient.GetTokenAsync(_realm, tokenRequest);
                _accessToken = response.AccessToken;
                _tokenExpiry = DateTime.UtcNow.AddSeconds(response.ExpiresIn - 60); // Subtract a minute as a buffer
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to authenticate with Keycloak", ex);
            }
        }
    }

    private string GetAuthToken()
    {
        return $"Bearer {_accessToken}";
    }

    #region User Management

    public async Task<IEnumerable<UserDto>> GetUsersAsync()
    {
        await EnsureValidTokenAsync();
        try
        {
            var keycloakUsers = await keycloakClient.GetUsersAsync(GetAuthToken(), _realm);
            return keycloakUsers.Select(u => u.ToDto());
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to get users from Keycloak", ex);
        }
    }

    public async Task<UserDto?> GetUserByIdAsync(string userId)
    {
        await EnsureValidTokenAsync();
        try
        {
            var user = await keycloakClient.GetUserByIdAsync(GetAuthToken(), _realm, userId);
            return user.ToDto();
        }
        catch (Refit.ApiException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to get user with ID {userId} from Keycloak", ex);
        }
    }

    public async Task<UserDto?> GetUserByUsernameAsync(string username)
    {
        await EnsureValidTokenAsync();
        try
        {
            var users = await keycloakClient.GetUsersByUsernameAsync(GetAuthToken(), _realm, username);
            return users.FirstOrDefault(u => u.Username == username)?.ToDto();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to get user with username {username} from Keycloak", ex);
        }
    }

    public async Task<bool> CreateUserAsync(UserDto userDto, string password)
    {
        await EnsureValidTokenAsync();

        var user = userDto.ToModel();

        user.Credentials.Add(new KeycloakCredential
        {
            Type = "password",
            Value = password,
            Temporary = false
        });

        try
        {
            await keycloakClient.CreateUserAsync(GetAuthToken(), _realm, user);
            return true;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to create user in Keycloak", ex);
        }
    }

    public async Task<bool> UpdateUserAsync(string userId, UserDto user)
    {
        await EnsureValidTokenAsync();
        try
        {
            await keycloakClient.UpdateUserAsync(GetAuthToken(), _realm, userId, user.ToModel());
            return true;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to update user with ID {userId} in Keycloak", ex);
        }
    }

    public async Task<bool> DeleteUserAsync(string userId)
    {
        await EnsureValidTokenAsync();
        try
        {
            await keycloakClient.DeleteUserAsync(GetAuthToken(), _realm, userId);
            return true;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to delete user with ID {userId} from Keycloak", ex);
        }
    }

    public async Task<bool> ResetPasswordAsync(string userId, CredentialDto credential)
    {
        await EnsureValidTokenAsync();
        try
        {
            await keycloakClient.ResetPasswordAsync(GetAuthToken(), _realm, userId, credential.ToModel());
            return true;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to reset password for user with ID {userId} in Keycloak", ex);
        }
    }

    #endregion

    #region Role Management

    public async Task<IEnumerable<RoleDto>> GetRolesAsync()
    {
        await EnsureValidTokenAsync();
        try
        {
            var roles = await keycloakClient.GetRolesAsync(GetAuthToken(), _realm);
            return roles.Select(r => r.ToDto());
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to get roles from Keycloak", ex);
        }
    }

    public async Task<RoleDto?> GetRoleByNameAsync(string roleName)
    {
        await EnsureValidTokenAsync();
        try
        {
            var role = await keycloakClient.GetRoleByNameAsync(GetAuthToken(), _realm, roleName);
            return role.ToDto();
        }
        catch (Refit.ApiException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to get role with name {roleName} from Keycloak", ex);
        }
    }

    public async Task<bool> AssignRolesToUserAsync(string userId, List<string> roles)
    {
        await EnsureValidTokenAsync();
        try
        {
            var allRoles = await GetRolesAsync();
            var rolesToAdd = allRoles.Where(r => roles.Contains(r.Name ?? string.Empty))
                .Select(r => r.ToModel()).ToList();
            
            if (rolesToAdd.Any())
            {
                await keycloakClient.AddRealmRolesToUserAsync(GetAuthToken(), _realm, userId, rolesToAdd);
            }
            
            return true;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to assign roles to user with ID {userId} in Keycloak", ex);
        }
    }

    public async Task<bool> RemoveRolesFromUserAsync(string userId, List<string> roles)
    {
        await EnsureValidTokenAsync();
        try
        {
            var userRoles = await GetUserRolesAsync(userId);
            var rolesToRemove = userRoles.Where(r => roles.Contains(r.Name ?? string.Empty))
                .Select(r => r.ToModel()).ToList();
            
            if (rolesToRemove.Any())
            {
                await keycloakClient.RemoveRealmRolesFromUserAsync(GetAuthToken(), _realm, userId, rolesToRemove);
            }
            
            return true;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to remove roles from user with ID {userId} in Keycloak", ex);
        }
    }

    public async Task<IEnumerable<RoleDto>> GetUserRolesAsync(string userId)
    {
        await EnsureValidTokenAsync();
        try
        {
            var roles = await keycloakClient.GetUserRealmRolesAsync(GetAuthToken(), _realm, userId);
            return roles.Select(r => r.ToDto());
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to get roles for user with ID {userId} from Keycloak", ex);
        }
    }

    #endregion

    #region Group Management

    public async Task<IEnumerable<GroupDto>> GetGroupsAsync()
    {
        await EnsureValidTokenAsync();
        try
        {
            var groups = await keycloakClient.GetGroupsAsync(GetAuthToken(), _realm);
            return groups.Select(g => g.ToDto());
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to get groups from Keycloak", ex);
        }
    }

    public async Task<GroupDto?> GetGroupByIdAsync(string groupId)
    {
        await EnsureValidTokenAsync();
        try
        {
            var group = await keycloakClient.GetGroupByIdAsync(GetAuthToken(), _realm, groupId);
            return group.ToDto();
        }
        catch (Refit.ApiException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to get group with ID {groupId} from Keycloak", ex);
        }
    }

    public async Task<GroupDto?> GetGroupByPathAsync(string groupPath)
    {
        var groups = await GetGroupsAsync();
        return FindGroupByPath(groups, groupPath);
    }

    private static GroupDto? FindGroupByPath(IEnumerable<GroupDto> groups, string path)
    {
        foreach (var group in groups)
        {
            if (group.Path == path)
            {
                return group;
            }

            if (group.SubGroups != null && group.SubGroups.Any())
            {
                var subGroupMatch = FindGroupByPath(group.SubGroups, path);
                if (subGroupMatch != null)
                {
                    return subGroupMatch;
                }
            }
        }

        return null;
    }

    public async Task<bool> AssignUserToGroupsAsync(string userId, List<string> groupIds)
    {
        await EnsureValidTokenAsync();
        try
        {
            foreach (var groupId in groupIds)
            {
                await keycloakClient.AddUserToGroupAsync(GetAuthToken(), _realm, userId, groupId);
            }
            
            return true;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to assign user with ID {userId} to groups in Keycloak", ex);
        }
    }

    public async Task<bool> RemoveUserFromGroupsAsync(string userId, List<string> groupIds)
    {
        await EnsureValidTokenAsync();
        try
        {
            foreach (var groupId in groupIds)
            {
                await keycloakClient.RemoveUserFromGroupAsync(GetAuthToken(), _realm, userId, groupId);
            }
            
            return true;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to remove user with ID {userId} from groups in Keycloak", ex);
        }
    }

    public async Task<IEnumerable<string>> GetUserGroupsAsync(string userId)
    {
        await EnsureValidTokenAsync();
        try
        {
            var groups = await keycloakClient.GetUserGroupsAsync(GetAuthToken(), _realm, userId);
            return groups.Select(g => g.Id ?? string.Empty).Where(id => !string.IsNullOrEmpty(id));
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to get groups for user with ID {userId} from Keycloak", ex);
        }
    }

    #endregion
} 