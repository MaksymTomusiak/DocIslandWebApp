using Domain.Files;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using File = Domain.Files.File;

namespace Infrastructure.Persistence.Configurations;

public class FileConfigurator : IEntityTypeConfiguration<File>
{
    public void Configure(EntityTypeBuilder<File> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, x => new FileId(x));
        
        builder.Property(x => x.OriginalFileName)
            .HasColumnType("varchar(500)").IsRequired();

        builder.Property(x => x.FileSizeBytes)
            .IsRequired();

        builder.HasOne(x => x.User)
            .WithMany(x => x.Files)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}