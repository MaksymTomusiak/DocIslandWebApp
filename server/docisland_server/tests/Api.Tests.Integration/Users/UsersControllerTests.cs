using System.Net;
using Domain.Roles;
using Domain.Users;
using FluentAssertions;
using Tests.Common;
using Tests.Data;

namespace Api.Tests.Integration.Users;

public class UsersControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly User _mainUser;
    private readonly Role _userRole = RolesData.UserRole;
    private readonly Role _adminRole = RolesData.AdminRole;
    private const string TestPassword = "TestPass123!";
    
    public UsersControllerTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _mainUser = UsersData.MainUser();
    }

    [Fact]
    public async Task ShouldToggleUserAdmin()
    {
        // Arrange
        var userId = _mainUser.Id;
        
        // Act
        var response = await Client.PostAsync($"users/{userId}/toggle-admin", null);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        (await UserManager.IsInRoleAsync(_mainUser, _userRole.Name!)).Should().BeFalse();
        (await UserManager.IsInRoleAsync(_mainUser, _adminRole.Name!)).Should().BeTrue();
    }
    
    [Fact]
    public async Task ShouldNotToggleUserAdminBecauseNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        
        // Act
        var response = await Client.PostAsync($"users/{userId}/toggle-admin", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldToggleUserBan()
    {
        // Arrange
        var userId = _mainUser.Id;
        
        // Act
        var response = await Client.PostAsync($"users/{userId}/toggle-ban", null);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        (await UserManager.FindByIdAsync(_mainUser.Id))?.IsBanned.Should().BeTrue();
    }
    
    [Fact]
    public async Task ShouldNotToggleUserBanBecauseNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        
        // Act
        var response = await Client.PostAsync($"users/{userId}/toggle-ban", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    public async Task InitializeAsync()
    {
        await RoleManager.CreateAsync(_userRole);
        await UserManager.CreateAsync(_mainUser, TestPassword);
        await SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        Context.Users.RemoveRange(Context.Users);
        Context.Roles.RemoveRange(Context.Roles);
        await SaveChangesAsync();
    }
}