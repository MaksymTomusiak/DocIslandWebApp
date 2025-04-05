using Domain.Messages;
using Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class MessageConfigurator : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, x => new MessageId(x));
        
        builder.Property(x => x.CreatedAt)
            .HasConversion(new DateTimeUtcConverter());
        
        builder.Property(x => x.Content)
            .HasColumnType("varchar(10000)").IsRequired();
        
        builder.HasOne(x => x.Conversation)
            .WithMany(x => x.Messages)
            .HasForeignKey(x => x.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.IsResponse).IsRequired();
    }
}