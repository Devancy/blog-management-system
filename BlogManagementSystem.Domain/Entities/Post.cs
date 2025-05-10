namespace BlogManagementSystem.Domain.Entities;

public class Post
{
	public Guid Id { get; set; }
	public string Title { get; set; } = string.Empty;
	public string Slug { get; set; } = string.Empty;
	public string Content { get; set; } = string.Empty;
	public string AuthorId { get; set; } = string.Empty; // KeyCloak user ID
	public PostStatus Status { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }
	public List<Comment> Comments { get; set; } = [];
}

public enum PostStatus
{
	Draft,
	Submitted,
	Approved,
	Published
}