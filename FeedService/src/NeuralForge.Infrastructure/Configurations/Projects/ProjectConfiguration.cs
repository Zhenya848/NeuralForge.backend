using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeuralForge.Domain.Projects;
using NeuralForge.Domain.Projects.ValueObjects.Ids;

namespace NeuralForge.Infrastructure.Configurations.Projects;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("projects");
        
        builder.Property(i => i.Id)
            .HasConversion(i => i.Value, value => ProjectId.Create(value));
        
        builder.Property(u => u.UserId).IsRequired();
        builder.Property(n => n.Name).IsRequired().HasMaxLength(50);
        builder.Property(d => d.Description).IsRequired().HasMaxLength(250);
        builder.Property(e => e.EmojiId).IsRequired();
        builder.Property(c => c.ColorId).IsRequired();
        builder.Property(i => i.IsSystem).IsRequired();
        
        builder.HasMany(c => c.ChatFolders).WithOne().HasForeignKey(i => i.ProjectId);
        builder.HasMany(f => f.FragmentFolders).WithOne().HasForeignKey(i => i.ProjectId);
    }
}