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
    public Task<ChatFolder> GetDefaultFolder(Guid userId, CancellationToken cancellationToken = default);
    public Task<bool> HasFolderWithName(
        Guid userId, 
        string name, 
        CancellationToken cancellationToken = default, 
        FolderId? parentId = null);
}