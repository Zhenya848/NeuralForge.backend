using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeuralForge.Domain.Chats;
using NeuralForge.Domain.Chats.ValueObjects.Ids;
using NeuralForge.Domain.Folders.ValueObjects.Ids;

namespace NeuralForge.Infrastructure.Configurations.Chats;

public class ChatConfiguration : IEntityTypeConfiguration<Chat>
{
    public void Configure(EntityTypeBuilder<Chat> builder)
    {
        builder.ToTable("chats");
        
        builder.Property(i => i.Id)
            .HasConversion(i => i.Value, value => ChatId.Create(value));
        
        builder.Property(i => i.FolderId)
            .HasConversion(i => i.Value, value => FolderId.Create(value));
        
        builder.Property(i => i.UserId).IsRequired();
        builder.Property(n => n.Name).IsRequired();
        
        builder.HasMany(m => m.Messages).WithOne().HasForeignKey(c => c.ChatId);
        
        builder.Property(c => c.Connections)
            .HasConversion(
                v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                json => JsonSerializer.Deserialize<List<ChatId>>(json, JsonSerializerOptions.Default)!,
                
                new ValueComparer<IReadOnlyCollection<ChatId>>(
                    (c1, c2) => c1!.SequenceEqual(c2!),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),

                    c => new List<ChatId>(c)
                )
            )
            .HasColumnType("jsonb");
    }
}