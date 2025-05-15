using System;
using System.ComponentModel.DataAnnotations;

namespace BlogManagementSystem.Domain.Entities;

/// <summary>
/// Represents a configuration setting stored in the database.
/// </summary>
public class AppSetting
{
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Key { get; set; } = string.Empty;
    
    [Required]
    public string Value { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
}