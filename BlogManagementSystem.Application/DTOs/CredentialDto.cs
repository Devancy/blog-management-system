namespace BlogManagementSystem.Application.DTOs;

public class CredentialDto
{
	public string Type { get; set; } = "password";
	public string? Value { get; set; }
	public bool Temporary { get; set; } = false;
}