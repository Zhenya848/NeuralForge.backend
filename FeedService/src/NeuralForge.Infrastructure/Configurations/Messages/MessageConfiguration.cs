using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeuralForge.Domain.Chats.Messages;
using NeuralForge.Domain.Chats.ValueObjects.Ids;

namespace NeuralForge.Infrastructure.Configurations.Messages;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable("messages");
        
        builder.HasDiscriminator<string>("Type")
            .HasValue<ClientMessage>("client")
            .HasValue<AiMessage>("ai");
        
        builder.Property(i => i.Id)
            .HasConversion(i => i.Value, value => MessageId.Create(value));
        
        builder.Property(i => i.ChatId)
            .HasConversion(i => i.Value, value => ChatId.Create(value));
        
        builder.Property(sn => sn.SerialNumber).IsRequired();
        builder.Property(v => v.Value).IsRequired();
        
        builder.Property(c => c.CreatedAt).IsRequired();
        builder.Property(ib => ib.IsBookmarked).IsRequired();
    }
}