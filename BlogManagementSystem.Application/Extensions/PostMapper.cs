using BlogManagementSystem.Application.DTOs;
using BlogManagementSystem.Domain.Entities;

namespace BlogManagementSystem.Application.Extensions;

public static class PostMapper
{
	public static PostDto ToDto(this Post post,
		string authorName = "",
		string authorEmail = "",
		List<CommentDto>? comments = null)
	{
		return new PostDto
		{
			Id = post.Id,
			Title = post.Title,
			Slug = post.Slug,
			Content = post.Content,
			Status = post.Status,
			CreatedAt = post.CreatedAt,
			UpdatedAt = post.UpdatedAt,
			AuthorId = post.AuthorId,
			AuthorName = authorName,
			AuthorEmail = authorEmail,
			Comments = comments ?? []
		};
	}

}