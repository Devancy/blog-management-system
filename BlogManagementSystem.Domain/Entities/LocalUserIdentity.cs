using System;
using System.Collections.Generic;

namespace BlogManagementSystem.Domain.Entities;

public class LocalUserIdentity
{
    public string UserId { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Organization { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime LastLoginAt { get; set; }
    
    // Navigation properties
    public virtual ICollection<LocalUserRole> UserRoles { get; set; } = new List<LocalUserRole>();
    public virtual ICollection<LocalUserGroup> UserGroups { get; set; } = new List<LocalUserGroup>();
} 