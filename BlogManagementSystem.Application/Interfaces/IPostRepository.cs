using BlogManagementSystem.Domain.Entities;

namespace BlogManagementSystem.Application.Interfaces;

public interface IPostRepository
{
	Task<Post> CreateAsync(Post post, CancellationToken cancellationToken);
	Task<Post?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
	Task<List<Post>> GetAllAsync(CancellationToken cancellationToken);
	Task UpdateAsync(Post post, CancellationToken cancellationToken);
	Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}