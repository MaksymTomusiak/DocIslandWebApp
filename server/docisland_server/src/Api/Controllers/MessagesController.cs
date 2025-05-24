using Api.Dtos;
using Api.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.Messages.Commands;
using Domain.Conversations;
using Domain.Messages;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("messages")]
[Authorize(AuthenticationSchemes = "Clerk")]
public class MessagesController(ISender sender, IMessageQueries messageQueries) : ControllerBase
{
    [HttpGet("conversation/{conversationId:guid}")]
    public async Task<IEnumerable<MessageDto>> GetAll(Guid conversationId, CancellationToken cancellationToken)
    {
        var entities = await messageQueries.GetByConversationId(new ConversationId(conversationId), cancellationToken);
        
        return entities.Select(MessageDto.FromDomainModel);
    }
    
    [HttpGet("{messageId:guid}")]
    public async Task<ActionResult<MessageDto>> GetById(Guid messageId, CancellationToken cancellationToken)
    {
        var entity = await messageQueries.GetById(new MessageId(messageId), cancellationToken);
        return entity.Match<ActionResult<MessageDto>>(
            m => MessageDto.FromDomainModel(m),
            () => NotFound());
    }
    
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