using System.Text.Json.Serialization;

namespace BlogManagementSystem.Domain.Entities;

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
    public List<KeycloakCredential>? Credentials { get; set; }
    
    [JsonPropertyName("realmRoles")]
    public List<string>? RealmRoles { get; set; }
    
    [JsonPropertyName("groups")]
    public List<string>? Groups { get; set; }
    
    [JsonPropertyName("emailVerified")]
    public bool EmailVerified { get; set; } = false;
    
    [JsonPropertyName("attributes")]
    public Dictionary<string, List<string>>? Attributes { get; set; }
}

public class KeycloakCredential
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "password";
    
    [JsonPropertyName("value")]
    public string? Value { get; set; }
    
    [JsonPropertyName("temporary")]
    public bool Temporary { get; set; } = false;
}

public class KeycloakRole
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }
    
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    
    [JsonPropertyName("description")]
    public string? Description { get; set; }
    
    [JsonPropertyName("composite")]
    public bool Composite { get; set; }
    
    [JsonPropertyName("clientRole")]
    public bool ClientRole { get; set; }
    
    [JsonPropertyName("containerId")]
    public string? ContainerId { get; set; }
}

public class KeycloakGroup
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }
    
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    
    [JsonPropertyName("path")]
    public string? Path { get; set; }
    
    [JsonPropertyName("subGroups")]
    public List<KeycloakGroup>? SubGroups { get; set; }
    
    [JsonIgnore]
    public bool IsExpanded { get; set; } = true;
} 