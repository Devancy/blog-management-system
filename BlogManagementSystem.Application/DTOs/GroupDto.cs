namespace BlogManagementSystem.Application.DTOs;

public class GroupDto
{
	public string? Id { get; set; }
	public string? Name { get; set; }
	public string? Path { get; set; }
	public string? ParentGroupId { get; set; }
	public List<GroupDto>? SubGroups { get; set; }
	public bool IsExpanded { get; set; } = true;
}