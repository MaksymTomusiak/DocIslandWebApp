using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Conversations;
using LanguageExt;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class ConversationRepository(ApplicationDbContext context) : IConversationRepository, IConversationQueries
{
    public async Task<IReadOnlyList<Conversation>> GetAll(CancellationToken cancellationToken)
    {
        return await context.Conversations
            .AsNoTracking()
            .Include(x => x.User)
            .Include(x => x.File)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Conversation>> GetRecentByUser(string userId, CancellationToken cancellationToken, int limit = 2)
    {
        return await context.Conversations
            .Include(x => x.File)
            .Include(x => x.Messages)
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.Messages
                .Where(m => m.IsResponse)
                .Max(m => m.CreatedAt))
            .Take(limit)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    
    public async Task<IReadOnlyList<Conversation>> GetByUser(string userId, CancellationToken cancellationToken)
    {
        return await context.Conversations
            .Include(x => x.File)
            .Include(x => x.Messages)
            .Where(x => x.UserId == userId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Option<Conversation>> GetById(ConversationId id, CancellationToken cancellationToken)
    {
        var entity = await context.Conversations
            .Include(x => x.File)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        
        return entity == null ? Option<Conversation>.None: Option<Conversation>.Some(entity);
    }

    public async Task<Conversation> Add(Conversation conversation, CancellationToken cancellationToken)
    {
        await context.Conversations.AddAsync(conversation, cancellationToken);
        
        await context.SaveChangesAsync(cancellationToken);
        
        return conversation;
    }

    public async Task<Conversation> Update(Conversation conversation, CancellationToken cancellationToken)
    {
        context.Conversations.Update(conversation);
        
        await context.SaveChangesAsync(cancellationToken);
        
        return conversation;
    }

    public async Task<Conversation> Delete(Conversation conversation, CancellationToken cancellationToken)
    {
        context.ChangeTracker.Clear();
        
        context.Conversations.Remove(conversation);
        
        await context.SaveChangesAsync(cancellationToken);

        return conversation;
    }
}