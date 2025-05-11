using BlogManagementSystem.Application.Interfaces;
using BlogManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlogManagementSystem.Infrastructure.Persistence;

public class PostRepository(ApplicationDbContext context) : IPostRepository
{
	public async Task<Post> CreateAsync(Post post, CancellationToken cancellationToken)
	{
		post.Id = Guid.NewGuid();
		post.CreatedAt = DateTime.UtcNow;
		context.Posts.Add(post);
		await context.SaveChangesAsync(cancellationToken);
		return post;
	}

	public async Task<Post?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
	{
		return await context.Posts
			.Include(p => p.Comments)
			.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
	}

	public async Task<Post?> GetBySlugAsync(string slug, CancellationToken cancellationToken)
	{
		return await context.Posts
			.Include(p => p.Comments)
			.FirstOrDefaultAsync(p => p.Slug == slug, cancellationToken);
	}

	public async Task<List<Post>> GetAllAsync(CancellationToken cancellationToken)
	{
		return await context.Posts
			.Include(p => p.Comments)
			.ToListAsync(cancellationToken);
	}

	public async Task UpdateAsync(Post post, CancellationToken cancellationToken)
	{
		post.UpdatedAt = DateTime.UtcNow;
		context.Posts.Update(post);
		await context.SaveChangesAsync(cancellationToken);
	}

	public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
	{
		var post = await GetByIdAsync(id, cancellationToken);
		if (post != null)
		{
			context.Posts.Remove(post);
			await context.SaveChangesAsync(cancellationToken);
		}
	}
}