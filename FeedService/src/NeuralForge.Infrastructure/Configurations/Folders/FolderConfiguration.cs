using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeuralForge.Domain.Folders;
using NeuralForge.Domain.Folders.ValueObjects.Ids;
using NeuralForge.Domain.Projects.ValueObjects.Ids;

namespace NeuralForge.Infrastructure.Configurations.Folders;

public class FolderConfiguration : IEntityTypeConfiguration<Folder>
{
    public void Configure(EntityTypeBuilder<Folder> builder)
    {
        builder.Property(i => i.Id)
            .HasConversion(i => i.Value, value => FolderId.Create(value));
        
        builder.Property(i => i.UserId).IsRequired();
        builder.Property(i => i.IsSystem).IsRequired();
        
        builder.Property(i => i.ParentId)
            .HasConversion(
                id => id == null ? (Guid?)null : id.Value,
                value => value == null ? null : FolderId.Create(value.Value))
            .IsRequired(false);

        builder.Property(n => n.Name).IsRequired();
        builder.Property(e => e.EmojiId).IsRequired();
    }
}