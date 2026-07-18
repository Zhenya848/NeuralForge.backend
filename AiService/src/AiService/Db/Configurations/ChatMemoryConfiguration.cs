using AiService.Models.Chats;
using AiService.Models.Chats.ValueObjects.Ids;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AiService.Db.Configurations;

public class ChatMemoryConfiguration : IEntityTypeConfiguration<ChatMemory>
{
    public void Configure(EntityTypeBuilder<ChatMemory> builder)
    {
        builder.ToTable("chat_memories");
        
        builder.Property(i => i.Id)
            .HasConversion(i => i.Value, value => ChatMemoryId.Create(value));
        
        builder.Property(i => i.ChatId)
            .HasConversion(i => i.Value, value => ChatId.Create(value));

        builder.Property(x => x.Content);
        
        builder.Property(x => x.Embedding)
            .HasColumnType("vector(1536)");
        
        builder.HasIndex(x => x.ChatId); 
    }
}