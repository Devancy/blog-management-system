using System.Security.Claims;
using BlogManagementSystem.Application.DTOs;
using BlogManagementSystem.Domain.Entities;

namespace BlogManagementSystem.Application.Common.Security;

public static class RolePermissions
{
    public const string AdminRole = "Admin";
    public const string AuthorRole = "Author";
    public const string EditorRole = "Editor";
    public const string ReaderRole = "Reader";
    
    public static readonly string[] ElevatedRoles = [AdminRole, AuthorRole, EditorRole];
    
    public static bool CanViewAnyPost(ClaimsPrincipal user)
    {
        return ElevatedRoles.Any(role => user.IsInRole(role));
    }
    
    public static bool CanEditPost(PostDto post, ClaimsPrincipal user)
    {
        if (user.IsInRole(AdminRole))
            return true;
            
        // Author can only edit their own posts
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        return user.IsInRole(AuthorRole) && post.AuthorId == userId;
    }
    
    public static bool CanSubmitPost(PostDto post, ClaimsPrincipal user)
    {
        return CanEditPost(post, user) && post.Status == PostStatus.Draft;
    }
    
    public static bool CanApprovePost(PostDto post, ClaimsPrincipal user)
    {
        return (user.IsInRole(AdminRole) || user.IsInRole(EditorRole)) 
               && post.Status == PostStatus.Submitted;
    }
    
    public static bool CanPublishPost(PostDto post, ClaimsPrincipal user)
    {
        return (user.IsInRole(AdminRole) || user.IsInRole(EditorRole))
               && post.Status == PostStatus.Approved;
    }
    
    public static bool IsPostVisibleToUser(PostDto post, ClaimsPrincipal user)
    {
        if (CanViewAnyPost(user))
            return true;
            
        // For all other users (including readers and anonymous), only published and approved posts are visible
        return post.Status is PostStatus.Published or PostStatus.Approved;
    }
}