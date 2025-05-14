using System;

namespace BlogManagementSystem.Domain.Entities;

public class LocalGroupRole
{
    public Guid GroupId { get; set; }
    public Guid RoleId { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Navigation properties
    public virtual LocalGroup Group { get; set; } = null!;
    public virtual LocalRole Role { get; set; } = null!;
} 