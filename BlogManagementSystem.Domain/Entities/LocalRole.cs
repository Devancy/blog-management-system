using System;
using System.Collections.Generic;

namespace BlogManagementSystem.Domain.Entities;

public class LocalRole
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public virtual ICollection<LocalUserRole> UserRoles { get; set; } = new List<LocalUserRole>();
} 