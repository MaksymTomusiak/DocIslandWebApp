using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Messages;
using LanguageExt;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class MessageRepository(ApplicationDbContext context) : IMessageRepository, IMessageQueries
{
    public async Task<IReadOnlyList<Message>> GetAll(CancellationToken cancellationToken)
    {
        return await context.Messages
            .AsNoTracking()
            .Include(x => x.Conversation)
            .ToListAsync(cancellationToken);
    }
    
    public async Task<Option<Message>> GetById(MessageId id, CancellationToken cancellationToken)
    {
        var entity = await context.Messages
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        
        return entity == null ? Option<Message>.None: Option<Message>.Some(entity);
    }
    
    public async Task<Message> Add(Message message, CancellationToken cancellationToken)
    {
        await context.Messages.AddAsync(message, cancellationToken);
        
        await context.SaveChangesAsync(cancellationToken);
        
        return message;
    }
    
    public async Task<Message> Update(Message message, CancellationToken cancellationToken)
    {
        context.Messages.Update(message);
        
        await context.SaveChangesAsync(cancellationToken);
        
        return message;
    }
    
    public async Task<Message> Delete(Message message, CancellationToken cancellationToken)
    {
        context.Messages.Remove(message);
        
        await context.SaveChangesAsync(cancellationToken);

        return message;
    }
}