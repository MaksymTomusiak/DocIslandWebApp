using Api.Dtos;
using Api.Extensions;
using Api.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.Conversations.Commands;
using Domain.Conversations;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("conversations")]
[Authorize(AuthenticationSchemes = "Clerk")]
public class ConversationsController(
    ISender sender,
    IConversationQueries conversationQueries) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<ConversationDto>> GetAll(CancellationToken cancellationToken)
    {
        var entities = await conversationQueries.GetAll(cancellationToken);
        return entities.Select(ConversationDto.FromDomainModel);
    }
    
    [HttpGet("user")]
    public async Task<IEnumerable<ConversationDto>> GetAllByUser( CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        if (string.IsNullOrWhiteSpace(userId))
            throw new UnauthorizedAccessException("User ID not found in token.");

        var entities = await conversationQueries.GetByUser(userId, cancellationToken);
        return entities.Select(ConversationDto.FromDomainModel);
    }

    [HttpGet("{conversationId:guid}")]
    public async Task<ActionResult<ConversationDto>> GetById(Guid conversationId, CancellationToken cancellationToken)
    {
        var entity = await conversationQueries.GetById(new ConversationId(conversationId), cancellationToken);
        return entity.Match<ActionResult<ConversationDto>>(
            c => ConversationDto.FromDomainModel(c),
            () => NotFound());
    }
    
    [HttpPost("add")]
    public async Task<ActionResult<ConversationDto>> Create(
        [FromForm] ConversationCreateDto request,
        CancellationToken cancellationToken)
    {
        var input = new CreateConversationCommand
        {
            File = request.File
        };
        var result = await sender.Send(input, cancellationToken);
        return result.Match<ActionResult<ConversationDto>>(
            c => ConversationDto.FromDomainModel(c),
            e => e.ToObjectResult());
    }
    
    [HttpDelete("delete/{id:guid}")]
    public async Task<ActionResult<ConversationDto>> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var input = new DeleteConversationCommand
        {
            ConversationId = id
        };
        var result = await sender.Send(input, cancellationToken);
        return result.Match<ActionResult<ConversationDto>>(
            c => ConversationDto.FromDomainModel(c),
            e => e.ToObjectResult());
    }
}