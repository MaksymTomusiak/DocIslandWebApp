using Api.Dtos;
using Api.Modules.Errors;
using Application.Roles.Commands;
using Domain.Roles;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[Authorize(Roles = "Admin")]
[Route("roles")]
[ApiController]
public class RolesController(RoleManager<Role> roleManager, ISender sender): ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<RoleDto>> GetAll(CancellationToken cancellationToken = default)
    {
        var roles = await roleManager.Roles.ToListAsync(cancellationToken);
        
        return roles.Select(r => RoleDto.FromDomainModel(r));
    }
    
    [HttpPost("add")]
    public async Task<ActionResult<RoleDto>> Add([FromBody] CreateRoleDto createRoleDto, CancellationToken cancellationToken = default)
    {
        var command = new CreateRoleCommand()
        {
            Name = createRoleDto.Name, 
            Description = createRoleDto.Description
        };

        var result = await sender.Send(command);
        
        return result.Match<ActionResult<RoleDto>>(
            r => RoleDto.FromDomainModel(r),
            e => e.ToObjectResult());
    }
    
    [HttpDelete("delete/{roleId:guid}")]
    public async Task<ActionResult> Delete([FromRoute] Guid roleId, CancellationToken cancellationToken = default)
    {
        var command = new DeleteRoleCommand()
        {
            RoleId = roleId
        };

        var result = await sender.Send(command);

        return result.Match(Ok, e => e.ToObjectResult());
    }
}