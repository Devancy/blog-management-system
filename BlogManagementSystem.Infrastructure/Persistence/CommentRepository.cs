using BlogManagementSystem.Application.Interfaces;
using BlogManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlogManagementSystem.Infrastructure.Persistence;

public class CommentRepository(ApplicationDbContext context) : ICommentRepository
{
	public async Task<Comment> CreateAsync(Comment comment, CancellationToken cancellationToken)
	{
		comment.Id = Guid.NewGuid();
		comment.CreatedAt = DateTime.UtcNow;
		context.Comments.Add(comment);
		await context.SaveChangesAsync(cancellationToken);
		return comment;
	}

	public async Task<Comment?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
	{
		return await context.Comments
			.Include(c => c.Post)
			.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
	}

	public async Task<List<Comment>> GetByPostIdAsync(Guid postId, CancellationToken cancellationToken)
	{
		return await context.Comments
			.Where(c => c.PostId == postId)
			.ToListAsync(cancellationToken);
	}

	public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
	{
		var comment = await GetByIdAsync(id, cancellationToken);
		if (comment != null)
		{
			context.Comments.Remove(comment);
			await context.SaveChangesAsync(cancellationToken);
		}
	}
}