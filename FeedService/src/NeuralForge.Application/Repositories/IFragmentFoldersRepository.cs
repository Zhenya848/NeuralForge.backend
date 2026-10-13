using CSharpFunctionalExtensions;
using NeuralForge.Domain.Folders;
using NeuralForge.Domain.Folders.ValueObjects.Ids;
using NeuralForge.Domain.Projects;
using NeuralForge.Domain.Projects.ValueObjects.Ids;
using NeuralForge.Domain.Shared;

namespace NeuralForge.Application.Repositories;

public interface IFragmentFoldersRepository : IRepository
{
    public Guid Add(FragmentFolder folder);
    public Guid Delete(FragmentFolder folder);
    public Task<Result<FragmentFolder, ErrorList>> GetById(
        Guid userId, 
        FolderId id, 
        CancellationToken cancellationToken = default);
    public Task<bool> IsFolderExist(Guid userId, ProjectId projectId, FolderId id, CancellationToken cancellationToken = default);
    
    public Task<FragmentFolder> GetDefaultFolder(Guid userId, CancellationToken cancellationToken = default);
}