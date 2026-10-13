using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using NeuralForge.Application.Repositories;
using NeuralForge.Domain.Folders.ValueObjects.Ids;
using NeuralForge.Domain.Fragments;
using NeuralForge.Domain.Fragments.ValueObjects.Ids;
using NeuralForge.Domain.Projects.ValueObjects.Ids;
using NeuralForge.Domain.Shared;
using NeuralForge.Infrastructure.DbContexts;

namespace NeuralForge.Infrastructure.Repositories;

public class FragmentsRepository : IFragmentsRepository
{
    private readonly AppDbContext _dbContext;

    public FragmentsRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Guid Add(Fragment fragment)
    {
        _dbContext.Fragments.Add(fragment);
        
        return fragment.Id;
    }

    public Guid Delete(Fragment chat)
    {
        _dbContext.Fragments.Remove(chat);
        
        return chat.Id;
    }

    public async Task<Result<Fragment, ErrorList>> GetById(
        Guid userId, 
        FragmentId fragmentId, 
        CancellationToken cancellationToken = default)
    {
        var fragment = await _dbContext.Fragments
            .FirstOrDefaultAsync(c =>
                    c.Id == fragmentId &&
                    c.UserId == userId,
                cancellationToken);

        if (fragment is null)
            return (ErrorList)Errors.General.NotFound(fragmentId);
        
        return fragment;
    }
    
    public async Task<Result<FolderId, ErrorList>> ResolveFolderIdForFragment(
        Guid userId, ProjectId? projectId, FolderId? folderId, CancellationToken cancellationToken)
    {
        if (folderId is not null)
        {
            var exists = await _dbContext.FragmentFolders.AnyAsync(
                f => f.UserId == userId && f.Id == folderId,
                cancellationToken);

            if (!exists)
                return (ErrorList)Errors.General.NotFound(folderId);

            return folderId;
        }
        
        if (projectId is null)
        {
            projectId = await _dbContext.Projects
                .Where(p => p.UserId == userId && p.IsSystem)
                .Select(p => p.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (projectId is null)
                return (ErrorList)Errors.General.NotFound();
        }
        
        var systemFolderId = await _dbContext.ChatFolders
            .Where(f =>
                f.UserId == userId &&
                f.ProjectId == projectId &&
                f.IsSystem)
            .Select(f => f.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (systemFolderId is null)
            return (ErrorList)Errors.General.NotFound();

        return systemFolderId;
    }
}