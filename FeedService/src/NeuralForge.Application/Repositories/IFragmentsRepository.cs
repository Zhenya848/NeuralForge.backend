using CSharpFunctionalExtensions;
using NeuralForge.Domain.Chats;
using NeuralForge.Domain.Chats.ValueObjects.Ids;
using NeuralForge.Domain.Folders.ValueObjects.Ids;
using NeuralForge.Domain.Fragments;
using NeuralForge.Domain.Fragments.ValueObjects.Ids;
using NeuralForge.Domain.Projects.ValueObjects.Ids;
using NeuralForge.Domain.Shared;

namespace NeuralForge.Application.Repositories;

public interface IFragmentsRepository : IRepository
{
    public Guid Delete(Fragment chat);
    public Task<Result<Fragment, ErrorList>> GetById(
        Guid userId,
        FragmentId fragmentId, 
        CancellationToken cancellationToken = default);
}