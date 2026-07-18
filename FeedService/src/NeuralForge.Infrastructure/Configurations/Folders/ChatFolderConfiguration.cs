using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeuralForge.Domain.Folders;
using NeuralForge.Domain.Projects.ValueObjects.Ids;

namespace NeuralForge.Infrastructure.Configurations.Folders;

public class ChatFolderConfiguration : IEntityTypeConfiguration<ChatFolder>
{
    public void Configure(EntityTypeBuilder<ChatFolder> builder)
    {
        builder.ToTable("chat_folders");
        
        builder.Property(i => i.ProjectId)
            .HasConversion(i => i.Value, value => ProjectId.Create(value));
        
        builder.HasMany(c => c.Chats).WithOne().HasForeignKey(i => i.FolderId);
    }
}