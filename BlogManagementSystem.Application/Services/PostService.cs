using BlogManagementSystem.Application.DTOs;
using BlogManagementSystem.Application.Extensions;
using BlogManagementSystem.Application.Interfaces;
using BlogManagementSystem.Domain.Entities;

namespace BlogManagementSystem.Application.Services;

public class PostService(
    IPostRepository postRepository,
    ICommentRepository commentRepository,
    IKeycloakService keycloakService)
{
    #region Entity Methods

    public async Task<Post> CreatePostAsync(string title, string slug, string content, string authorId, CancellationToken cancellationToken)
    {
        // Ensure slug is unique
        slug = await EnsureUniqueSlugAsync(slug, null, cancellationToken);
        
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

    public async Task<Post?> GetPostBySlugAsync(string slug, CancellationToken cancellationToken)
    {
        return await postRepository.GetBySlugAsync(slug, cancellationToken);
    }

    public async Task<List<Post>> GetAllPostsAsync(CancellationToken cancellationToken)
    {
        return await postRepository.GetAllAsync(cancellationToken);
    }

    public async Task UpdatePostAsync(Guid id, string title, string slug, string content, CancellationToken cancellationToken)
    {
        var post = await postRepository.GetByIdAsync(id, cancellationToken);
        if (post == null) throw new Exception("Post not found");

        // Ensure slug is unique but allow keeping the same slug for this post
        post.Title = title;
        post.Slug = await EnsureUniqueSlugAsync(slug, id, cancellationToken);
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
    
    #endregion

    #region DTO Methods

    // Get a single post with author information
    public async Task<PostDto?> GetPostDtoAsync(Guid id, CancellationToken cancellationToken)
    {
        var post = await postRepository.GetByIdAsync(id, cancellationToken);
        if (post == null) return null;
        
        return await EnrichPostWithAuthorInfoAsync(post, cancellationToken);
    }
    
    // Get a single post by slug with author information
    public async Task<PostDto?> GetPostDtoBySlugAsync(string slug, CancellationToken cancellationToken)
    {
        var post = await postRepository.GetBySlugAsync(slug, cancellationToken);
        if (post == null) return null;
        
        return await EnrichPostWithAuthorInfoAsync(post, cancellationToken);
    }
    
    // Get all posts with author information
    public async Task<List<PostDto>> GetAllPostDtosAsync(CancellationToken cancellationToken)
    {
        var posts = await postRepository.GetAllAsync(cancellationToken);
        var postDtos = new List<PostDto>();
        
        foreach (var post in posts)
        {
            postDtos.Add(await EnrichPostWithAuthorInfoAsync(post, cancellationToken));
        }
        
        return postDtos;
    }
    
    // Get comments for a post with user information
    public async Task<List<CommentDto>> GetCommentDtosAsync(Guid postId, CancellationToken cancellationToken)
    {
        var comments = await commentRepository.GetByPostIdAsync(postId, cancellationToken);
        var commentDtos = new List<CommentDto>();
        
        foreach (var comment in comments)
        {
            commentDtos.Add(await EnrichCommentWithUserInfoAsync(comment, cancellationToken));
        }
        
        return commentDtos;
    }

    #endregion
    
    #region Helper Methods
    
    private async Task<string> EnsureUniqueSlugAsync(string slug, Guid? excludePostId, CancellationToken cancellationToken)
    {
        var baseSlug = slug;
        var counter = 2;
        var isUnique = false;
        
        while (!isUnique)
        {
            var existingPost = await postRepository.GetBySlugAsync(slug, cancellationToken);
            
            // If no post with this slug exists, or the only post with this slug is the one being updated
            if (existingPost == null || (excludePostId.HasValue && existingPost.Id == excludePostId.Value))
            {
                isUnique = true;
            }
            else
            {
                // Append a counter to make the slug unique
                slug = $"{baseSlug}-{counter}";
                counter++;
            }
        }
        
        return slug;
    }
    
    private async Task<PostDto> EnrichPostWithAuthorInfoAsync(Post post, CancellationToken cancellationToken)
    {
        string authorName = "";
        string authorEmail = "";
        
        try
        {
            var author = await keycloakService.GetUserByIdAsync(post.AuthorId);
            if (author != null)
            {
                authorName = GetFormattedUserName(author);
                authorEmail = author.Email ?? "";
            }
        }
        catch (Exception)
        {
            // If there's an error getting the author info, just use default values
        }
        
        var comments = await GetCommentDtosAsync(post.Id, cancellationToken);
        
        return post.ToDto(authorName, authorEmail, comments);
    }
    
    private async Task<CommentDto> EnrichCommentWithUserInfoAsync(Comment comment, CancellationToken cancellationToken)
    {
        string userName = "";
        string userEmail = "";
        
        try
        {
            var user = await keycloakService.GetUserByIdAsync(comment.UserId);
            if (user != null)
            {
                userName = GetFormattedUserName(user);
                userEmail = user.Email ?? "";
            }
        }
        catch (Exception)
        {
            // If there's an error getting the user info, just use default values
        }
        
        return comment.ToDto(userName, userEmail);
    }
    
    private static string GetFormattedUserName(UserDto user)
    {
        if (!string.IsNullOrEmpty(user.FirstName) && !string.IsNullOrEmpty(user.LastName))
        {
            return $"{user.FirstName} {user.LastName}";
        }

        if (!string.IsNullOrEmpty(user.FirstName))
        {
            return user.FirstName;
        }

        if (!string.IsNullOrEmpty(user.Username))
        {
            return user.Username;
        }

        return "Unknown User";
    }
    
    #endregion
}