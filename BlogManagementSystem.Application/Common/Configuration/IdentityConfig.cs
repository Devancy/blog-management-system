namespace BlogManagementSystem.Application.Common.Configuration;

public class IdentityConfig
{
    /// <summary>
    /// When true, Keycloak acts as a proxy for MS Entra ID and the application manages roles and groups locally.
    /// When false, Keycloak is the primary identity provider and manages users, roles, and groups.
    /// </summary>
    public bool UseKeycloakAsIdpProxy { get; set; }
} 