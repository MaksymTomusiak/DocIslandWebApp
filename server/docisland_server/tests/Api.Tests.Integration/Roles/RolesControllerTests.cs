using System.Net;
using System.Net.Http.Json;
using Api.Dtos;
using Domain.Roles;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Tests.Data;

namespace Api.Tests.Integration.Roles;

public class RolesControllerTests: BaseIntegrationTest, IAsyncLifetime
{
    private readonly Role _testRole;
    
    public RolesControllerTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _testRole = RolesData.TestRole;
    }

    [Fact]
    public async Task ShouldCreateRole()
    {
        // Arrange
        var request = new Role(Guid.NewGuid().ToString(), "NewRole", "NewRoleDescription");
        
        // Act
        var response = await Client.PostAsJsonAsync("roles/add", request);
        
        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        
        var responseMessage = await response.ToResponseModel<RoleDto>();

        var roleId = responseMessage.Id;

        var dbRole = await Context.Roles.FirstOrDefaultAsync(x => x.Id == roleId);

        dbRole.Should().NotBeNull();
        dbRole!.Name.Should().Be(request.Name);
        dbRole.Description.Should().Be(request.Description);
    }
    
    [Fact]
    public async Task ShouldNotCreateRoleBecauseAlreadyExists()
    {
        // Arrange
        var request = new Role(Guid.NewGuid().ToString(), _testRole.Name!, _testRole.Description);
        
        // Act
        var response = await Client.PostAsJsonAsync("roles/add", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task ShouldDeleteRole()
    {
        // Arrange
        var roleId = _testRole.Id;

        // Act
        var response = await Client.DeleteAsync($"roles/delete/{roleId}");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var dbRole = await Context.Roles.FirstOrDefaultAsync(x => x.Id == roleId);
        dbRole.Should().BeNull();
    }
    
    [Fact]
    public async Task ShouldNotDeleteRoleBecauseDoesNotExist()
    {
        // Arrange
        var roleId = Guid.NewGuid();

        // Act
        var response = await Client.DeleteAsync($"roles/delete/{roleId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    public async Task InitializeAsync()
    {
        await RoleManager.CreateAsync(_testRole);
        await SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        Context.Users.RemoveRange(Context.Users);
        Context.Roles.RemoveRange(Context.Roles);
        await SaveChangesAsync();
    }
}