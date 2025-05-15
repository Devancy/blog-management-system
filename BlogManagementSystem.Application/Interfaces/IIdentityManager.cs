using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BlogManagementSystem.Application.DTOs;
using BlogManagementSystem.Application.Interfaces.Identity;

namespace BlogManagementSystem.Application.Interfaces;

/// <summary>
/// Abstract interface for identity management operations that works in both modes:
/// 1. Keycloak as primary IDP (direct management of users, roles, and groups in Keycloak)
/// 2. Keycloak as IDP Proxy (users from MS Entra ID, local management of roles and groups)
/// </summary>
public interface IIdentityManager : 
    IUserManagement,
    IRoleManagement,
    IUserRoleManagement,
    IGroupManagement,
    IUserGroupManagement,
    IGroupRoleManagement,
    IIdentitySynchronization
{
    // This interface is now a composition of capability-focused interfaces
}