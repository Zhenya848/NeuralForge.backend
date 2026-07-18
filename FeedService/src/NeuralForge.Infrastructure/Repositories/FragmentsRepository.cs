using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using NeuralForge.Application.Repositories;
using NeuralForge.Domain.Fragments;
using NeuralForge.Domain.Fragments.ValueObjects.Ids;
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
}