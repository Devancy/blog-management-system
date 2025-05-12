namespace BlogManagementSystem.Application.DTOs;

public class RoleDto
{
	public string? Id { get; set; }
	public string? Name { get; set; }
	public string? Description { get; set; }
	public bool Composite { get; set; }
	public bool ClientRole { get; set; }
	public string? ContainerId { get; set; }
}