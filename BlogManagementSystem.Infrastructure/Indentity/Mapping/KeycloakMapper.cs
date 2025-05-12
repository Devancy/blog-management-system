using BlogManagementSystem.Application.DTOs;
using BlogManagementSystem.Infrastructure.Indentity.Models;

namespace BlogManagementSystem.Infrastructure.Indentity.Mapping;

internal static class KeycloakMapper
{
    public static UserDto ToDto(this KeycloakUser user)
    {
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Enabled = user.Enabled,
            EmailVerified = user.EmailVerified,
            Roles = user.RealmRoles,
            Groups = user.Groups,
            Attributes = user.Attributes
        };
    }

    public static KeycloakUser ToModel(this UserDto dto, string? password = null)
    {
        var user = new KeycloakUser
        {
            Id = dto.Id,
            Username = dto.Username,
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Enabled = dto.Enabled,
            EmailVerified = dto.EmailVerified,
            RealmRoles = dto.Roles,
            Groups = dto.Groups,
            Attributes = dto.Attributes
        };

        if (password != null)
        {
            user.Credentials = new List<KeycloakCredential>
            {
                new() { Value = password }
            };
        }

        return user;
    }

    public static RoleDto ToDto(this KeycloakRole role)
    {
        return new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            Composite = role.Composite,
            ClientRole = role.ClientRole,
            ContainerId = role.ContainerId
        };
    }

    public static KeycloakRole ToModel(this RoleDto dto)
    {
        return new KeycloakRole
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            Composite = dto.Composite,
            ClientRole = dto.ClientRole,
            ContainerId = dto.ContainerId
        };
    }

    public static GroupDto ToDto(this KeycloakGroup group)
    {
        return new GroupDto
        {
            Id = group.Id,
            Name = group.Name,
            Path = group.Path,
            IsExpanded = group.IsExpanded,
            SubGroups = group.SubGroups?.Select(g => g.ToDto()).ToList()
        };
    }

    public static KeycloakGroup ToModel(this GroupDto dto)
    {
        return new KeycloakGroup
        {
            Id = dto.Id,
            Name = dto.Name,
            Path = dto.Path,
            IsExpanded = dto.IsExpanded,
            SubGroups = dto.SubGroups?.Select(g => g.ToModel()).ToList()
        };
    }

    public static CredentialDto ToDto(this KeycloakCredential credential)
    {
        return new CredentialDto
        {
            Type = credential.Type,
            Value = credential.Value,
            Temporary = credential.Temporary
        };
    }

    public static KeycloakCredential ToModel(this CredentialDto dto)
    {
        return new KeycloakCredential
        {
            Type = dto.Type,
            Value = dto.Value,
            Temporary = dto.Temporary
        };
    }
}