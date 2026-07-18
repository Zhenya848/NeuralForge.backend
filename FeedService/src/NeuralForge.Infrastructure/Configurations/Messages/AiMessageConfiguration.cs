using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeuralForge.Domain.Chats.Messages;

namespace NeuralForge.Infrastructure.Configurations.Messages;

public class AiMessageConfiguration : IEntityTypeConfiguration<AiMessage>
{
    public void Configure(EntityTypeBuilder<AiMessage> builder)
    {
        builder.ComplexProperty(m => m.AiModel, mb =>
        {
            mb.Property(n => n.Name).IsRequired();
        });
    }
}