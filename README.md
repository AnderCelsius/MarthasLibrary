# Martha's Library System

## Description
Martha's Library System is an innovative .NET Core application that modernizes library management. The system allows library customers to interact with the library's catalog online, providing features such as book searching, reservation, and borrowing.

## Features
- Book search and reservation system
- Customer account management
- Notifications for book availability
- Secure authentication and authorization

## Technologies
- .NET Core
- Entity Framework Core
- Duende Identity Server
- Blazor Server
- Poly
- Nswag (For API documentation)

## Architecture
This Application uses Vertical Slice Architecture.

## Getting Started
I have left a copy of the `appsettings.json` in this repository, so you don't need any environment variables since there are no real secrets.
If you're using Visual Studio for development, ensure you have the 2022 version with .Net 8 installed as bulk on the development is done in .Net 8

## Migration
Open Package Manager Console and run the following commands
1. **IdentityServer project:**
   - Set the Default Project to MarthasLibrary.IdentityServer
   - Set the Startup Project to MarthasLibrary.IdentityServer
   - Update ConfigurationsDb by running the command  
    ```bash
    update-database -Context ConfigurationDbContext
    ```
   - Update the PersistedDBGrantDb by running the command 
    ```bash
    update-database -Context PersistedGrantDbContext
    ```
   - Update the IdentityDb by running the command 
    ```bash
    update-database -Context ApplicationDbContext
    ```
2. **API project:**
   - Set the Default Project to MarthasLibrary.Infrastructure
   - Set the Startup Project to MarthasLibrary.API
   - Update the LibraryDb by running the command 
    ```bash
    update-database
    ```

If you are using the .NET Core CLI (Command-Line Interface)
1. **IdentityServer project**:
   - Navigate to the IdentityServer project directory:
     ```bash
     cd path/to/MarthasLibrary.IdentityServer
     ```
   - Update the `ConfigurationDbContext`:
     ```bash
     dotnet ef database update --context ConfigurationDbContext
     ```
   - Update the `PersistedGrantDbContext`:
     ```bash
     dotnet ef database update --context PersistedGrantDbContext
     ```
   - Update the `ApplicationDbContext`:
     ```bash
     dotnet ef database update --context ApplicationDbContext
     ```

2. **API project**:
   - Navigate to the API project directory:
     ```bash
     cd path/to/MarthasLibrary.Infrastructure
     ```
   - Update the `LibraryDb`:
     ```bash
     dotnet ef database update
     ```

Note: 
- Ensure that you have the .NET Core SDK and the EF Core CLI tools installed in your system. You can install the EF Core CLI tools globally by running `dotnet tool install --global dotnet-ef`.
- Replace `path/to/project` with the actual path to your project directory.
- If your database context classes are located in a different project than your startup project, you might need to specify the startup project as well using the `--startup-project` option.

## Usage
Setup multiple projects in Visual Studio or create a Rider Configuration if you are using Rider.
The startup projects should include
- MarthasLibrary.API
- MarthasLibrary.IdentityServer
- MarthasLibrary.BlazorApp

**Test Users Credentials**
Customer: 
- Username: alice
- Password: Pass123$

Admin:
- Username: obai
- Password: Pass123$



