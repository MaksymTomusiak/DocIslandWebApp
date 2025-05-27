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

namespace Api.Tests.Integration.Users;

public class UsersControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    
    public UsersControllerTests(IntegrationTestWebFactory factory) : base(factory)
    {
    }
    

    public async Task InitializeAsync()
    {
        
    }
    
    public async Task DisposeAsync()
    {
       
    }
}