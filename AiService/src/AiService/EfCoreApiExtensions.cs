using System.Text.Json;
using AiService.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AiService
{
    public static class EfCoreApiExtensions
    {
        public static PropertyBuilder<TCollection> HasJsonCollectionConversion<TCollection>(
            this PropertyBuilder<TCollection> builder) 
            where TCollection : class, System.Collections.IEnumerable
        {
            return builder.HasConversion(
                value => JsonSerializer.Serialize(value, JsonSerializerOptions.Default),
                json => JsonSerializer.Deserialize<TCollection>(json, JsonSerializerOptions.Default)!,
                
                new ValueComparer<TCollection>(
                    (c1, c2) => c1 == null && c2 == null || c1 != null && c2 != null && c1.Cast<object>()
                        .SequenceEqual(c2.Cast<object>()),
                    
                    c => c.Cast<object>().Aggregate(0, (a, v) => HashCode.Combine(a, v == null ? 0 : v.GetHashCode())),
                    
                    c => JsonSerializer.Deserialize<TCollection>(JsonSerializer
                        .Serialize(c, JsonSerializerOptions.Default), JsonSerializerOptions.Default)!
                )
            ).HasColumnType("jsonb");
        }

        public static async Task ApplyMigrations(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
                
            try
            {
                var context = services.GetRequiredService<AppDbContext>();
                var migrations = await context.Database.GetPendingMigrationsAsync();
                    
                if (migrations.Any())
                {
                    await context.Database.MigrateAsync();
                    Console.WriteLine("🚀 Новые миграции успешно применены!");
                }
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while seeding the database.");
            }
        }
    }
}
