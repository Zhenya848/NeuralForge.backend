using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeuralForge.Domain.Folders;
using NeuralForge.Domain.Projects.ValueObjects.Ids;

namespace NeuralForge.Infrastructure.Configurations.Folders;

public class FragmentFolderConfiguration : IEntityTypeConfiguration<FragmentFolder>
{
    public void Configure(EntityTypeBuilder<FragmentFolder> builder)
    {
        builder.ToTable("fragment_folders");
        
        builder.Property(i => i.ProjectId)
            .HasConversion(i => i.Value, value => ProjectId.Create(value));
        
        builder.HasMany(f => f.Fragments).WithOne().HasForeignKey(i => i.FolderId);
    }
}