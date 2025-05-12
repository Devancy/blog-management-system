namespace BlogManagementSystem.Application.DTOs;

public class UserDto
{
	public string? Id { get; set; }
	public string? Username { get; set; }
	public bool Enabled { get; set; } = true;
	public string? Email { get; set; }
	public string? FirstName { get; set; }
	public string? LastName { get; set; }
	public List<string>? Roles { get; set; }
	public List<string>? Groups { get; set; }
	public bool EmailVerified { get; set; } = false;
	public Dictionary<string, List<string>>? Attributes { get; set; }
}