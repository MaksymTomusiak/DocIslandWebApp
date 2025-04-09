using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;
using Domain.Roles;
using Domain.Users;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Tests.Data;

namespace Api.Tests.Intagration.Users;

public class AuthControllerTests  : BaseIntegrationTest, IAsyncLifetime
{
    private readonly User _mainUser;
    private readonly User _secondaryUser;
    private readonly Role _userRole = RolesData.UserRole;
    private const string TestPassword = "TestPass123!";
    
    public AuthControllerTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _mainUser = UsersData.MainUser();
        _secondaryUser = UsersData.SecondaryUser();
    }
    
    [Fact]
    public async Task ShouldRegisterUser()
    {
        // Arrange
        const string userName = "testUserName";
        const string userEmail = "testUser@gmail.com";
        const string password = "TestPass123!";
        var request = new
        {
            UserName = userName,
            Email = userEmail,
            Password = password
        };

        // Act
        var response = await Client.PostAsJsonAsync("auth/register", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        
        var dbUser = await Context.Users.FirstOrDefaultAsync(x => x.Email == userEmail);
        dbUser.Should().NotBeNull();
        dbUser!.UserName.Should().Be(userName);
        dbUser.Email.Should().Be(userEmail);
        var isUserInRole = await UserManager.IsInRoleAsync(dbUser, "User");
        isUserInRole.Should().BeTrue();
        UserManager.PasswordHasher.VerifyHashedPassword(dbUser, dbUser.PasswordHash!, password)
            .Should().Be(PasswordVerificationResult.Success);
        
        var handler = new JwtSecurityTokenHandler();
        var responseToken = await response.Content.ReadAsStringAsync();
        responseToken.Should().NotBeNullOrEmpty();

        var token = handler.ReadJwtToken(responseToken);
        token.Should().NotBeNull();
        token.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == dbUser.Id.ToString());
        token.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Email && c.Value == dbUser.Email);
        token.Claims.Should().Contain(c => c.Type == "role" && c.Value == "User");
    }
    
    [Fact]
    public async Task ShouldNotRegisterBecauseEmailIsAlreadyUsed()
    {
        // Arrange
        const string userName = "testUserName";
        var userEmail = _mainUser.Email;
        const string password = "TestPass123!";
        var request = new
        {
            UserName = userName,
            Email = userEmail,
            Password = password
        };

        // Act
        var response = await Client.PostAsJsonAsync("auth/register", request);

        // Assert 
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task ShouldNotRegisterBecauseUserNameIsAlreadyUsed()
    {
        // Arrange
        var userName = _mainUser.UserName;
        const string userEmail = "testUser@gmail.com";
        const string password = "TestPass123!";
        var request = new
        {
            UserName = userName,
            Email = userEmail,
            Password = password
        };

        // Act
        var response = await Client.PostAsJsonAsync("auth/register", request);

        // Assert 
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task ShouldLoginUser()
    {
        // Arrange
        var userEmail = _mainUser.Email;
        var password = TestPassword;
        var request = new
        {
            Email = userEmail,
            Password = password
        };

        // Act
        var response = await Client.PostAsJsonAsync("auth/login", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var handler = new JwtSecurityTokenHandler();
        var responseToken = await response.Content.ReadAsStringAsync();
        responseToken.Should().NotBeNullOrEmpty();

        var token = handler.ReadJwtToken(responseToken);
        token.Should().NotBeNull();
        token.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == _mainUser.Id.ToString());
        token.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Email && c.Value == userEmail);
        token.Claims.Should().Contain(c => c.Type == "role" && c.Value == "User");
    }
    
    [Fact]
    public async Task ShouldNotLoginUserBecauseWrongPassword()
    {
        // Arrange
        var userEmail = _mainUser.Email;
        const string password = "WrongPass123!";
        var request = new
        {
            Email = userEmail,
            Password = password
        };

        // Act
        var response = await Client.PostAsJsonAsync("auth/login", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task ShouldNotLoginUserBecauseWrongEmail()
    {
        // Arrange
        const string userEmail = "notexistinguser@gmail.com";
        const string password = "SomePass123!";
        var request = new
        {
            Email = userEmail,
            Password = password
        };

        // Act
        var response = await Client.PostAsJsonAsync("auth/login", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    public async Task InitializeAsync()
    {
        await RoleManager.CreateAsync(_userRole);
        await UserManager.CreateAsync(_mainUser, TestPassword);
        await UserManager.CreateAsync(_secondaryUser, TestPassword);
        await UserManager.AddToRoleAsync(_mainUser, _userRole.Name!);
        await UserManager.AddToRoleAsync(_secondaryUser, _userRole.Name!);
        await SaveChangesAsync();
    }
    
    public async Task DisposeAsync()
    {
        Context.Users.RemoveRange(Context.Users);
        Context.Roles.RemoveRange(Context.Roles);
        await SaveChangesAsync();
    }
}