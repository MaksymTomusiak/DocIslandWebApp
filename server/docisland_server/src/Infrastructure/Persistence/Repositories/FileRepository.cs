using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Conversations;
using Domain.Files;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using File = Domain.Files.File;

namespace Infrastructure.Persistence.Repositories;

public class FileRepository(ApplicationDbContext context) : IFileQueries, IFileRepository
{
    public async Task<IReadOnlyList<File>> GetAll(CancellationToken cancellationToken)
    {
        return await context.Files
            .AsNoTracking()
            .Include(x => x.User)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<File>> GetByUser(Guid userId, CancellationToken cancellationToken)
    {
        return await context.Files
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Option<File>> GetByConversation(ConversationId conversationId, CancellationToken cancellationToken)
    {
        var conversation = await context.Conversations.FirstOrDefaultAsync(c => c.Id == conversationId, cancellationToken);
        if (conversation == null)
        {
            return Option<File>.None;
        }

        var entity = await context.Files
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == conversation.FileId, cancellationToken);
        
        return entity == null ? Option<File>.None: Option<File>.Some(entity);
    }

    public async Task<Option<File>> GetById(FileId id, CancellationToken cancellationToken)
    {
        var entity = await context.Files
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        
        return entity == null ? Option<File>.None: Option<File>.Some(entity);
    }

    public async Task<File> Add(File file, CancellationToken cancellationToken)
    {
        await context.Files.AddAsync(file, cancellationToken);
        
        await context.SaveChangesAsync(cancellationToken);
        
        return file;
    }

    public async Task<File> Update(File file, CancellationToken cancellationToken)
    {
        context.Files.Update(file);
        
        await context.SaveChangesAsync(cancellationToken);
        
        return file;
    }

    public async Task<File> Delete(File file, CancellationToken cancellationToken)
    {
        context.Files.Remove(file);
        
        await context.SaveChangesAsync(cancellationToken);

        return file;
    }
}