using System;

namespace BlogManagementSystem.Domain.Entities;

public class LocalUserGroup
{
    public string UserId { get; set; } = string.Empty;
    public Guid GroupId { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Navigation properties
    public virtual LocalGroup Group { get; set; } = null!;
    public virtual LocalUserIdentity? UserIdentity { get; set; }
} 