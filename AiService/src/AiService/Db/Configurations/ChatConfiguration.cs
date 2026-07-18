using System.Text.Json;
using AiService.Models;
using AiService.Models.Chats;
using AiService.Models.Chats.ValueObjects;
using AiService.Models.Chats.ValueObjects.Ids;
using AiService.Models.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AiService.Db.Configurations;

public class ChatConfiguration :  IEntityTypeConfiguration<Chat>
{
    public void Configure(EntityTypeBuilder<Chat> builder)
    {
        builder.ToTable("chats");
        
        builder.Property(i => i.Id)
            .HasConversion(i => i.Value, value => ChatId.Create(value));

        builder.Property(i => i.UserId).IsRequired();
        builder.Property(s => s.Summary).HasMaxLength(Constants.MAX_SUMMARY_TOKENS * 4);

        builder.Property(m => m.MessagesForSummarize).HasJsonCollectionConversion();
        builder.Property(m => m.AiModels).HasJsonCollectionConversion();
        
        builder.Property(c => c.LastMessages)
            .HasConversion(
                v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                v => JsonSerializer.Deserialize<Queue<Message>>(v, JsonSerializerOptions.Default) ?? new Queue<Message>(),
                
                new ValueComparer<IReadOnlyCollection<Message>>(
                    (c1, c2) => c1!.SequenceEqual(c2!),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),

                    c => new Queue<Message>(c)
                )
            )
            .HasColumnType("jsonb");
    }
}