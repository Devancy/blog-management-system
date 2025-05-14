using System;

namespace BlogManagementSystem.Domain.Entities;

public class LocalUserRole
{
    public string UserId { get; set; } = string.Empty;
    public Guid RoleId { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Navigation properties
    public virtual LocalRole Role { get; set; } = null!;
    public virtual LocalUserIdentity? UserIdentity { get; set; }
} 