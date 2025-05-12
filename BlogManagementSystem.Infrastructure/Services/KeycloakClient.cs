using BlogManagementSystem.Domain.Entities;
using Refit;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlogManagementSystem.Infrastructure.Services;

public interface IKeycloakAdminClient
{
    [Get("/admin/realms/{realm}/users")]
    Task<IEnumerable<KeycloakUser>> GetUsersAsync(
        [Header("Authorization")] string authToken,
        [AliasAs("realm")] string realm);
    
    [Get("/admin/realms/{realm}/users/{id}")]
    Task<KeycloakUser> GetUserByIdAsync(
        [Header("Authorization")] string authToken,
        [AliasAs("realm")] string realm,
        [AliasAs("id")] string userId);
    
    [Get("/admin/realms/{realm}/users")]
    Task<IEnumerable<KeycloakUser>> GetUsersByUsernameAsync(
        [Header("Authorization")] string authToken,
        [AliasAs("realm")] string realm,
        [AliasAs("username")] string username);
    
    [Post("/admin/realms/{realm}/users")]
    Task CreateUserAsync(
        [Header("Authorization")] string authToken,
        [AliasAs("realm")] string realm,
        [Body] KeycloakUser user);
    
    [Put("/admin/realms/{realm}/users/{id}")]
    Task UpdateUserAsync(
        [Header("Authorization")] string authToken,
        [AliasAs("realm")] string realm,
        [AliasAs("id")] string userId,
        [Body] KeycloakUser user);
    
    [Delete("/admin/realms/{realm}/users/{id}")]
    Task DeleteUserAsync(
        [Header("Authorization")] string authToken,
        [AliasAs("realm")] string realm,
        [AliasAs("id")] string userId);
    
    [Post("/admin/realms/{realm}/users/{id}/reset-password")]
    Task ResetPasswordAsync(
        [Header("Authorization")] string authToken,
        [AliasAs("realm")] string realm,
        [AliasAs("id")] string userId,
        [Body] KeycloakCredential credential);
    
    // Roles
    [Get("/admin/realms/{realm}/roles")]
    Task<IEnumerable<KeycloakRole>> GetRolesAsync(
        [Header("Authorization")] string authToken,
        [AliasAs("realm")] string realm);
    
    [Get("/admin/realms/{realm}/roles/{roleName}")]
    Task<KeycloakRole> GetRoleByNameAsync(
        [Header("Authorization")] string authToken,
        [AliasAs("realm")] string realm,
        [AliasAs("roleName")] string roleName);
    
    [Get("/admin/realms/{realm}/users/{id}/role-mappings/realm")]
    Task<IEnumerable<KeycloakRole>> GetUserRealmRolesAsync(
        [Header("Authorization")] string authToken,
        [AliasAs("realm")] string realm,
        [AliasAs("id")] string userId);
    
    [Post("/admin/realms/{realm}/users/{id}/role-mappings/realm")]
    Task AddRealmRolesToUserAsync(
        [Header("Authorization")] string authToken,
        [AliasAs("realm")] string realm,
        [AliasAs("id")] string userId,
        [Body] IEnumerable<KeycloakRole> roles);
    
    [Delete("/admin/realms/{realm}/users/{id}/role-mappings/realm")]
    Task RemoveRealmRolesFromUserAsync(
        [Header("Authorization")] string authToken,
        [AliasAs("realm")] string realm,
        [AliasAs("id")] string userId,
        [Body] IEnumerable<KeycloakRole> roles);
    
    // Groups
    [Get("/admin/realms/{realm}/groups")]
    Task<IEnumerable<KeycloakGroup>> GetGroupsAsync(
        [Header("Authorization")] string authToken,
        [AliasAs("realm")] string realm);
    
    [Get("/admin/realms/{realm}/groups/{id}")]
    Task<KeycloakGroup> GetGroupByIdAsync(
        [Header("Authorization")] string authToken,
        [AliasAs("realm")] string realm,
        [AliasAs("id")] string groupId);
    
    [Get("/admin/realms/{realm}/users/{id}/groups")]
    Task<IEnumerable<KeycloakGroup>> GetUserGroupsAsync(
        [Header("Authorization")] string authToken,
        [AliasAs("realm")] string realm,
        [AliasAs("id")] string userId);
    
    [Put("/admin/realms/{realm}/users/{id}/groups/{groupId}")]
    Task AddUserToGroupAsync(
        [Header("Authorization")] string authToken,
        [AliasAs("realm")] string realm,
        [AliasAs("id")] string userId,
        [AliasAs("groupId")] string groupId);
    
    [Delete("/admin/realms/{realm}/users/{id}/groups/{groupId}")]
    Task RemoveUserFromGroupAsync(
        [Header("Authorization")] string authToken,
        [AliasAs("realm")] string realm,
        [AliasAs("id")] string userId,
        [AliasAs("groupId")] string groupId);
    
    // Auth
    [Post("/realms/{realm}/protocol/openid-connect/token")]
    [Headers("Content-Type: application/x-www-form-urlencoded")]
    Task<KeycloakTokenResponse> GetTokenAsync(
        [AliasAs("realm")] string realm,
        [Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, string> form);
}

public class KeycloakTokenResponse
{
    [JsonPropertyName("access_token")]
    public string? AccessToken { get; set; }
    
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
    
    [JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; set; }
    
    [JsonPropertyName("refresh_expires_in")]
    public int RefreshExpiresIn { get; set; }
    
    [JsonPropertyName("token_type")]
    public string? TokenType { get; set; }
    
    [JsonPropertyName("scope")]
    public string? Scope { get; set; }
} 