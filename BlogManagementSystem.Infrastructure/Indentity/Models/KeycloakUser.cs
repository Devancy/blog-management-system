using System.Text.Json.Serialization;

namespace BlogManagementSystem.Infrastructure.Indentity.Models;

public class KeycloakUser
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }
    
    [JsonPropertyName("username")]
    public string? Username { get; set; }
    
    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; } = true;
    
    [JsonPropertyName("email")]
    public string? Email { get; set; }
    
    [JsonPropertyName("firstName")]
    public string? FirstName { get; set; }
    
    [JsonPropertyName("lastName")]
    public string? LastName { get; set; }
    
    [JsonPropertyName("credentials")]
    public List<KeycloakCredential> Credentials { get; set; } = [];
    
    [JsonPropertyName("realmRoles")]
    public List<string>? RealmRoles { get; set; }
    
    [JsonPropertyName("groups")]
    public List<string>? Groups { get; set; }
    
    [JsonPropertyName("emailVerified")]
    public bool EmailVerified { get; set; } = false;
    
    [JsonPropertyName("attributes")]
    public Dictionary<string, List<string>>? Attributes { get; set; } = [];
}