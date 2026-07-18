using AiService.Models;
using AiService.Models.Chats;
using AiService.Models.Chats.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace AiService.Db;

public class AppDbContext(IConfiguration configuration) : DbContext
{
    public DbSet<Chat> Chats => Set<Chat>();
    public DbSet<ChatMemory> ChatMemories => Set<ChatMemory>();
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(configuration.GetConnectionString("Database"), npgsqlOptions => 
        {
            npgsqlOptions.UseVector(); 
        });
        optionsBuilder.UseSnakeCaseNamingConvention();
        optionsBuilder.UseLoggerFactory(CreateLoggerFactory());
        optionsBuilder.EnableSensitiveDataLogging();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Ignore<Message>(); 
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.HasPostgresExtension("vector");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly); 
    }

    private ILoggerFactory CreateLoggerFactory() =>
        LoggerFactory.Create(b => b.AddConsole());
}