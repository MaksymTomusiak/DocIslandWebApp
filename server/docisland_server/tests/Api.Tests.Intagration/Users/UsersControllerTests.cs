using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;
using Api.Dtos;
using Domain.Roles;
using Domain.Users;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Tests.Data;

namespace Api.Tests.Intagration.Users;

public class UsersControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly User _mainUser;
    private readonly User _secondaryUser;
    private readonly User _testAdminUser;
    private readonly Role _userRole = RolesData.UserRole;
    private readonly Role _adminRole = RolesData.AdminRole;
    private const string TestPassword = "TestPass123!";
    
    public UsersControllerTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _mainUser = UsersData.MainUser();
        _secondaryUser = UsersData.SecondaryUser();
        _testAdminUser = UsersData.AdminUser();
    }

    
    
    [Fact]
    public async Task ShouldDeleteUsualUserByAdmin()
    {
        // Arrange
        var userId = _secondaryUser.Id;

        // Act
        var response = await Client.DeleteAsync($"users/delete/{userId}");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        
        var dbUser = await Context.Users.FirstOrDefaultAsync(x => x.Id == userId);
        dbUser.Should().BeNull();
    }

    [Fact]
    public async Task ShouldDeleteUsualUserByThemselves()
    {
        // Arrange
        var userId = _secondaryUser.Id;
        SetCustomAuthorizationHeader(JwtProvider.Generate(_secondaryUser, _userRole));

        // Act
        var response = await Client.DeleteAsync($"users/delete/{userId}");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        
        var dbUser = await Context.Users.FirstOrDefaultAsync(x => x.Id == userId);
        dbUser.Should().BeNull();
    }
    
    [Fact]
    public async Task ShouldDeleteAdminUserByThemselves()
    {
        // Arrange
        var userId = _testAdminUser.Id;
        SetCustomAuthorizationHeader(JwtProvider.Generate(_testAdminUser, _adminRole));

        // Act
        var response = await Client.DeleteAsync($"users/delete/{userId}");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        
        var dbUser = await Context.Users.FirstOrDefaultAsync(x => x.Id == userId);
        dbUser.Should().BeNull();
    }
    
    [Fact]
    public async Task ShouldNotDeleteUsualUserByAnotherUsualUser()
    {
        // Arrange
        var userId = _mainUser.Id;
        SetCustomAuthorizationHeader(JwtProvider.Generate(_secondaryUser, _userRole));

        // Act
        var response = await Client.DeleteAsync($"users/delete/{userId}");

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        
        var dbUser = await Context.Users.FirstOrDefaultAsync(x => x.Id == userId);
        dbUser.Should().NotBeNull();
    }
    
    [Fact]
    public async Task ShouldNotDeleteAdminByUsualUser()
    {
        // Arrange
        var userId = _testAdminUser.Id;
        SetCustomAuthorizationHeader(JwtProvider.Generate(_secondaryUser, _userRole));

        // Act
        var response = await Client.DeleteAsync($"users/delete/{userId}");

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        
        var dbUser = await Context.Users.FirstOrDefaultAsync(x => x.Id == userId);
        dbUser.Should().NotBeNull();
    }
    
    [Fact]
    public async Task ShouldNotDeleteAdminByAnotherAdminUser()
    {
        // Arrange
        var userId = _testAdminUser.Id;

        // Act
        var response = await Client.DeleteAsync($"users/delete/{userId}");

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        
        var dbUser = await Context.Users.FirstOrDefaultAsync(x => x.Id == userId);
        dbUser.Should().NotBeNull();
    }

    [Fact]
    public async Task ShouldUpdateUserPassword()
    {
        // Arrange
        var oldPassword = TestPassword;
        var newPassword = "NewPass123!";
        var request = new UserUpdatePasswordDto(oldPassword, newPassword);
        SetCustomAuthorizationHeader(JwtProvider.Generate(_mainUser, _userRole));

        // Act
        var response = await Client.PutAsJsonAsync("users/update-password", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var responseUser = await response.ToResponseModel<UserDto>();
        
        var dbUser = await Context.Users.FirstOrDefaultAsync(x => x.Id == responseUser.Id);
        dbUser.Should().NotBeNull();
        UserManager.PasswordHasher.VerifyHashedPassword(dbUser!, dbUser!.PasswordHash!, newPassword)
            .Should().Be(PasswordVerificationResult.Success);
    }

    [Fact]
    public async Task ShouldNotUpdateUserPasswordBecauseWrongOldPassword()
    {
        // Arrange
        var oldPassword = "WrongPass123!";
        var newPassword = "NewPass123!";
        var request = new UserUpdatePasswordDto(oldPassword, newPassword);
        SetCustomAuthorizationHeader(JwtProvider.Generate(_mainUser, _userRole));

        // Act
        var response = await Client.PutAsJsonAsync("users/update-password", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ShouldUpdateUserName()
    {
        // Arrange
        var newUserName = "UpdatedUserName";
        var request = new UserUpdateUserNameDto(newUserName);
        SetCustomAuthorizationHeader(JwtProvider.Generate(_mainUser, _userRole));

        // Act
        var response = await Client.PutAsJsonAsync("users/update-username", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var responseUser = await response.ToResponseModel<UserDto>();
        
        var dbUser = await Context.Users.FirstOrDefaultAsync(x => x.Id == responseUser.Id);
        dbUser.Should().NotBeNull();
        dbUser!.UserName.Should().Be(newUserName);
    }

    [Fact]
    public async Task ShouldNotUpdateUserNameBecauseAlreadyTaken()
    {
        // Arrange
        var newUserName = _secondaryUser.UserName;
        var request = new UserUpdateUserNameDto(newUserName!);
        SetCustomAuthorizationHeader(JwtProvider.Generate(_mainUser, _userRole));

        // Act
        var response = await Client.PutAsJsonAsync("users/update-username", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    public async Task InitializeAsync()
    {
        await RoleManager.CreateAsync(_userRole);
        await RoleManager.CreateAsync(_adminRole);
        await UserManager.CreateAsync(_mainUser, TestPassword);
        await UserManager.CreateAsync(_secondaryUser, TestPassword);
        await UserManager.CreateAsync(_testAdminUser, TestPassword);
        await UserManager.AddToRoleAsync(_mainUser, _userRole.Name!);
        await UserManager.AddToRoleAsync(_secondaryUser, _userRole.Name!);
        await UserManager.AddToRoleAsync(_testAdminUser, _adminRole.Name!);
        await SaveChangesAsync();
    }
    
    public async Task DisposeAsync()
    {
        Context.Users.RemoveRange(Context.Users);
        Context.Roles.RemoveRange(Context.Roles);
        await SaveChangesAsync();
    }
}