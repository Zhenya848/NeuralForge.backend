using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NeuralForge.Domain.Chats;
using NeuralForge.Domain.Folders;
using NeuralForge.Domain.Fragments;
using NeuralForge.Domain.Projects;

namespace NeuralForge.Infrastructure.DbContexts;

public class AppDbContext(IConfiguration configuration) : DbContext
{
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ChatFolder> ChatFolders => Set<ChatFolder>();
    public DbSet<Chat> Chats => Set<Chat>();
    public DbSet<FragmentFolder> FragmentFolders => Set<FragmentFolder>();
    public DbSet<Fragment> Fragments => Set<Fragment>();
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(configuration.GetConnectionString("Database"));
        optionsBuilder.UseSnakeCaseNamingConvention();
        optionsBuilder.UseLoggerFactory(CreateLoggerFactory());
        optionsBuilder.EnableSensitiveDataLogging();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Folder>()
            .UseTpcMappingStrategy();
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly); 
    }

    private ILoggerFactory CreateLoggerFactory() =>
        LoggerFactory.Create(b => b.AddConsole());
}