using System.Text.Json.Serialization;

namespace BlogManagementSystem.Infrastructure.Indentity.Models;

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