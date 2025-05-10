namespace BlogManagementSystem.Domain.Entities;

public class Comment
{
	public Guid Id { get; set; }
	public Guid PostId { get; set; }
	public string UserId { get; set; } = string.Empty; // KeyCloak user ID
	public string Content { get; set; } = string.Empty;
	public DateTime CreatedAt { get; set; }
	public Post Post { get; set; } = null!;
}