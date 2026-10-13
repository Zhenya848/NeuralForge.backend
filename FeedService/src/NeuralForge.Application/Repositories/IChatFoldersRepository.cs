using CSharpFunctionalExtensions;
using NeuralForge.Domain.Folders;
using NeuralForge.Domain.Folders.ValueObjects.Ids;
using NeuralForge.Domain.Projects.ValueObjects.Ids;
using NeuralForge.Domain.Shared;

namespace NeuralForge.Application.Repositories;

public interface IChatFoldersRepository : IRepository
{
    public Guid Create(ChatFolder chatFolder);
    public Guid Delete(ChatFolder chatFolder);
    public Task<Result<ChatFolder, ErrorList>> GetById(Guid userId, FolderId id, CancellationToken cancellationToken = default);
    public Task<bool> IsFolderExist(Guid userId, ProjectId projectId, FolderId id, CancellationToken cancellationToken = default);
    public Task<FolderId> GetDefaultFolderId(Guid userId, ProjectId? projectId, CancellationToken cancellationToken = default);
    public Task<bool> HasFolderWithName(
        Guid userId, 
        string name, 
        CancellationToken cancellationToken = default, 
        ProjectId? projectId = null,
        FolderId? parentId = null);
}