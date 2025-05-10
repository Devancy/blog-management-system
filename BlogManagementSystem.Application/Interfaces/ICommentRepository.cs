using BlogManagementSystem.Domain.Entities;

namespace BlogManagementSystem.Application.Interfaces;

public interface ICommentRepository
{
	Task<Comment> CreateAsync(Comment comment, CancellationToken cancellationToken);
	Task<Comment?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
	Task<List<Comment>> GetByPostIdAsync(Guid postId, CancellationToken cancellationToken);
	Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}