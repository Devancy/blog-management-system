using BlogManagementSystem.Application.Interfaces;
using BlogManagementSystem.Domain.Entities;

namespace BlogManagementSystem.Application.Services;

public class PostService(IPostRepository postRepository, ICommentRepository commentRepository)
{
    public async Task<Post> CreatePostAsync(string title, string slug, string content, string authorId, CancellationToken cancellationToken)
    {
        var post = new Post
        {
            Title = title,
            Slug = slug,
            Content = content,
            AuthorId = authorId,
            Status = PostStatus.Draft,
            CreatedAt = DateTime.UtcNow
        };
        return await postRepository.CreateAsync(post, cancellationToken);
    }

    public async Task<Post?> GetPostAsync(Guid id, CancellationToken cancellationToken)
    {
        return await postRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<List<Post>> GetAllPostsAsync(CancellationToken cancellationToken)
    {
        return await postRepository.GetAllAsync(cancellationToken);
    }

    public async Task UpdatePostAsync(Guid id, string title, string slug, string content, CancellationToken cancellationToken)
    {
        var post = await postRepository.GetByIdAsync(id, cancellationToken);
        if (post == null) throw new Exception("Post not found");

        post.Title = title;
        post.Slug = slug;
        post.Content = content;
        post.UpdatedAt = DateTime.UtcNow;

        await postRepository.UpdateAsync(post, cancellationToken);
    }

    public async Task DeletePostAsync(Guid id, CancellationToken cancellationToken)
    {
        await postRepository.DeleteAsync(id, cancellationToken);
    }

    public async Task<Comment> AddCommentAsync(Guid postId, string userId, string content, CancellationToken cancellationToken)
    {
        var post = await postRepository.GetByIdAsync(postId, cancellationToken);
        if (post == null) throw new Exception("Post not found");

        var comment = new Comment
        {
            PostId = postId,
            UserId = userId,
            Content = content,
            CreatedAt = DateTime.UtcNow
        };
        return await commentRepository.CreateAsync(comment, cancellationToken);
    }

    public async Task<List<Comment>> GetCommentsAsync(Guid postId, CancellationToken cancellationToken)
    {
        return await commentRepository.GetByPostIdAsync(postId, cancellationToken);
    }

    public async Task DeleteCommentAsync(Guid commentId, CancellationToken cancellationToken)
    {
        await commentRepository.DeleteAsync(commentId, cancellationToken);
    }
    
    // post management
    
    public async Task SubmitPostAsync(Guid id, CancellationToken cancellationToken)
    {
        var post = await postRepository.GetByIdAsync(id, cancellationToken);
        if (post == null) throw new Exception("Post not found");
        if (post.Status != PostStatus.Draft) throw new Exception("Post must be in Draft status");

        post.Status = PostStatus.Submitted;
        post.UpdatedAt = DateTime.UtcNow;
        await postRepository.UpdateAsync(post, cancellationToken);
    }

    public async Task ApprovePostAsync(Guid id, CancellationToken cancellationToken)
    {
        var post = await postRepository.GetByIdAsync(id, cancellationToken);
        if (post == null) throw new Exception("Post not found");
        if (post.Status != PostStatus.Submitted) throw new Exception("Post must be in Submitted status");

        post.Status = PostStatus.Approved;
        post.UpdatedAt = DateTime.UtcNow;
        await postRepository.UpdateAsync(post, cancellationToken);
    }

    public async Task PublishPostAsync(Guid id, CancellationToken cancellationToken)
    {
        var post = await postRepository.GetByIdAsync(id, cancellationToken);
        if (post == null) throw new Exception("Post not found");
        if (post.Status != PostStatus.Approved) throw new Exception("Post must be in Approved status");

        post.Status = PostStatus.Published;
        post.UpdatedAt = DateTime.UtcNow;
        await postRepository.UpdateAsync(post, cancellationToken);
    }
}