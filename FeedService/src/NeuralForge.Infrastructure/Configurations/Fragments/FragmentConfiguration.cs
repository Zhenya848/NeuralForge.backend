using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeuralForge.Domain.Chats.ValueObjects.Ids;
using NeuralForge.Domain.Folders.ValueObjects.Ids;
using NeuralForge.Domain.Fragments;
using NeuralForge.Domain.Fragments.ValueObjects.Ids;

namespace NeuralForge.Infrastructure.Configurations.Fragments;

public class FragmentConfiguration : IEntityTypeConfiguration<Fragment>
{
    public void Configure(EntityTypeBuilder<Fragment> builder)
    {
        builder.ToTable("fragments");
        
        builder.Property(i => i.Id)
            .HasConversion(i => i.Value, value => FragmentId.Create(value));
        
        builder.Property(i => i.FolderId)
            .HasConversion(i => i.Value, value => FolderId.Create(value));
        
        builder.Property(i => i.UserId).IsRequired();
        builder.Property(n => n.Name).IsRequired();
        builder.Property(c => c.Content).IsRequired();
        
        builder.HasOne(m => m.Message).WithMany().HasForeignKey("message_id");
    }
}