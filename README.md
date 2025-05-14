# Blog Management System

A PoC blog management system built with Blazor Server, following Clean Architecture principles, with MudBlazor UI components and Keycloak integration for identity management.

## Architecture

This solution implements Clean Architecture with the following layers:

- **Domain**: Contains business entities, interfaces, and domain logic
- **Application**: Contains business rules, commands/queries, and services
- **Infrastructure**: Implements data access, external service integrations, and persistence
- **Presentation**: Blazor Server application with MudBlazor UI components

## Technologies

- **.NET 8**: Latest .NET framework
- **Blazor Server**: Interactive web UI framework
- **MudBlazor**: Material Design component library for Blazor
- **Entity Framework Core**: ORM for database operations
- **Keycloak**: Identity and Access Management solution
- **PostgreSQL**: Database for storing application data
- **Docker**: Containerization for development and deployment

## Prerequisites

- Visual Studio 2022 or later
- .NET 8 SDK
- Docker Desktop
- Git

## Getting Started

### Clone the Repository

```bash
git clone https://github.com/Devancy/blog-management-system
cd blog-management-system
```

### Run with Docker

1. Ensure Docker Desktop is running
2. Run the application using Docker Compose:

```bash
docker-compose up -d
```

This will start:
- PostgreSQL database on port 5432
- Keycloak server on port 6060
- Blazor application

### Run with Visual Studio

1. Open `BlogManagementSystem.sln` in Visual Studio
2. Set the `BlogManagementSystem.Presentation` as the startup project
3. Start the required services using Docker:

```bash
docker-compose up -d blogmanagementsystem.keycloak blogmanagementsystem.postgres
```

4. Press F5 to run the application from Visual Studio

## Keycloak Configuration

The application is configured to use Keycloak as the identity provider.

- Keycloak Admin URL: http://localhost:6060
- Default credentials:
  - Username: admin
  - Password: admin

### Authentication Scenarios

The solution supports two authentication scenarios:

1. **Keycloak as the primary identity provider:**
   - Keycloak directly manages user identities, groups, and roles
   - User management and role assignments are handled within Keycloak
   - Application retrieves user information and permissions from Keycloak

2. **Keycloak as middleware (IDP Proxy):**
   - Keycloak proxies authentication via third-party identity providers (e.g., MS Entra)
   - External IDP handles user authentication
   - Groups and roles are managed within the application itself
   - Allows integration with existing enterprise identity infrastructure

### Initial Setup

1. Login to Keycloak admin console
2. Create a new realm for the application
3. Create a client for the application with the following settings:
   - Client ID: blog-app-client
   - Access Type: confidential
   - Valid Redirect URIs: https://localhost:5051/
4. Setup users, groups, and roles as needed

## Development Guidelines

### Clean Architecture

Follow the principles of Clean Architecture:
- Don't reference outward from inner layers
- The Domain should not have dependencies on other layers
- Application layer should only depend on Domain
- Infrastructure should implement interfaces defined in the Domain/Application
- Presentation should only interact with the Application layer

### Blazor and MudBlazor Components

- Use MudBlazor components for UI
- For generic components like MudChip, MudList, specify type arguments explicitly
- Follow component lifecycle best practices

### Database Migrations

To create a new migration:

```bash
dotnet ef migrations add MigrationName --project BlogManagementSystem.Infrastructure --startup-project BlogManagementSystem.Presentation
```

To update the database:

```bash
dotnet ef database update --project BlogManagementSystem.Infrastructure --startup-project BlogManagementSystem.Presentation
```

## Contributing

1. Follow the established architecture and coding standards
2. Write unit tests for new functionality
3. Ensure all tests pass before submitting pull requests

## Known Issues

### Docker Authentication Issue

When running the application with Docker, there is a known issue where the blog application cannot authenticate with the Keycloak container. This is due to networking configuration between containers. 

**Workaround:**
- Run Keycloak and PostgreSQL in Docker
- Run the Blazor application directly through Visual Studio

## License

MIT License

Copyright (c) 2024

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.