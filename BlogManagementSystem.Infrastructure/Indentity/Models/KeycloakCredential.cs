using System.Text.Json.Serialization;

namespace BlogManagementSystem.Infrastructure.Indentity.Models;

public class KeycloakCredential
{
	[JsonPropertyName("type")]
	public string Type { get; set; } = "password";
    
	[JsonPropertyName("value")]
	public string? Value { get; set; }
    
	[JsonPropertyName("temporary")]
	public bool Temporary { get; set; } = false;
}