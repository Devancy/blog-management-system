using System;
using System.Collections.Generic;

namespace BlogManagementSystem.Domain.Entities;

public class LocalGroup
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public Guid? ParentGroupId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public virtual LocalGroup? ParentGroup { get; set; }
    public virtual ICollection<LocalGroup> SubGroups { get; set; } = new List<LocalGroup>();
    public virtual ICollection<LocalUserGroup> UserGroups { get; set; } = new List<LocalUserGroup>();
} 