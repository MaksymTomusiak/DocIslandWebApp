using Api.Dtos;
using Api.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.Conversations.Commands;
using Domain.Conversations;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("conversations")]
public class ConversationsController(
    ISender sender,
    IConversationQueries conversationQueries) : ControllerBase
{
    [Authorize]
    [HttpGet]
    public async Task<IEnumerable<ConversationDto>> GetAll(CancellationToken cancellationToken)
    {
        var entities = await conversationQueries.GetAll(cancellationToken);

        return entities.Select(ConversationDto.FromDomainModel);
    }

    [Authorize]
    [HttpGet("{conversationId:guid}")]
    public async Task<ActionResult<ConversationDto>> GetById(Guid conversationId, CancellationToken cancellationToken)
    {
        var entity = await conversationQueries.GetById(new ConversationId(conversationId), cancellationToken);

        return entity.Match<ActionResult<ConversationDto>>(
            c => ConversationDto.FromDomainModel(c),
            () => NotFound());
    }
    
    [Authorize]
    [HttpPost("add")]
    public async Task<ActionResult<ConversationDto>> Create(
        [FromBody] ConversationCreateDto request,
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
    
    [Authorize]
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