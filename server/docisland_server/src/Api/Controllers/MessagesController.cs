using Api.Dtos;
using Api.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.Messages.Commands;
using Domain.Messages;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("messages")]
public class MessagesController(ISender sender, IMessageQueries messageQueries) : ControllerBase
{
    [Authorize]
    [HttpGet]
    public async Task<IEnumerable<MessageDto>> GetAll(CancellationToken cancellationToken)
    {
        var entities = await messageQueries.GetAll(cancellationToken);
        
        return entities.Select(MessageDto.FromDomainModel);
    }
    
    [Authorize]
    [HttpGet("{messageId:guid}")]
    public async Task<ActionResult<MessageDto>> GetById(Guid messageId, CancellationToken cancellationToken)
    {
        var entity = await messageQueries.GetById(new MessageId(messageId), cancellationToken);
        return entity.Match<ActionResult<MessageDto>>(
            m => MessageDto.FromDomainModel(m),
            () => NotFound());
    }
    
    [Authorize]
    [HttpPost("add")]
    public async Task<ActionResult<MessageDto>> Create([FromBody] MessageCreateDto messageCreateDto, CancellationToken cancellationToken)
    {
        var command = new CreateMessageCommand
        {
            Content = messageCreateDto.Content,
            ConversationId = messageCreateDto.ConversationId
        };
        var entity = await sender.Send(command, cancellationToken);
        return entity.Match<ActionResult<MessageDto>>(
            m => MessageDto.FromDomainModel(m), 
            e => e.ToObjectResult());
    }
    
    [Authorize]
    [HttpDelete("delete/{messageId:guid}")]
    public async Task<ActionResult<MessageDto>> Delete(Guid messageId, CancellationToken cancellationToken)
    {
        var command = new DeleteMessageCommand
        {
            MessageId = messageId
        };
        var entity = await sender.Send(command, cancellationToken);
        return entity.Match<ActionResult<MessageDto>>(
            m => MessageDto.FromDomainModel(m), 
            e => e.ToObjectResult());
    }
}