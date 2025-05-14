using BlogManagementSystem.Application.DTOs;
using BlogManagementSystem.Domain.Entities;

namespace BlogManagementSystem.Application.Extensions;

public static class CommentMapper
{
	public static CommentDto ToDto(this Comment comment, string userName = "", string userEmail = "")
	{
		return new CommentDto
		{
			Id = comment.Id,
			PostId = comment.PostId,
			Content = comment.Content,
			CreatedAt = comment.CreatedAt,
			UserId = comment.UserId,
			UserName = userName,
			UserEmail = userEmail
		};
	}
}