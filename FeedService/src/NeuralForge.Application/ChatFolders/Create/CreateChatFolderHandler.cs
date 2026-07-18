using CSharpFunctionalExtensions;
using NeuralForge.Application.Abstractions;
using NeuralForge.Application.Repositories;
using NeuralForge.Domain.Folders;
using NeuralForge.Domain.Folders.ValueObjects.Ids;
using NeuralForge.Domain.Projects;
using NeuralForge.Domain.Projects.ValueObjects.Ids;
using NeuralForge.Domain.Shared;

namespace NeuralForge.Application.ChatFolders.Create;

public class CreateChatFolderHandler : ICommandHandler<CreateChatFolderCommand, Result<Guid, ErrorList>>
{
    private readonly IProjectsRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateChatFolderHandler(
        IProjectsRepository repository, 
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Result<Guid, ErrorList>> Handle(
        CreateChatFolderCommand command, 
        CancellationToken cancellationToken = default)
    {
        Project project;
        
        if (command.ProjectId is not null)
        {
            var result = await _repository.GetById(
                command.UserId,
                ProjectId.Create(command.ProjectId.Value),
                cancellationToken);
            
            if (result.IsFailure)
                return result.Error;
            
            project = result.Value;
        }
        else
            project = await _repository.GetDefaultProject(command.UserId, cancellationToken);

        var parentId = command.ParentId is not null ? FolderId.Create(command.ParentId.Value) : null;
        var hasFolder = project.ChatFolders.Any(f => f.Name == command.Name && f.ParentId == parentId);

        if (hasFolder)
            return (ErrorList)Error.Conflict("folder.name.duplicate", "Folder with this name already exists");

        var createFolderResult = ChatFolder.Create(
            command.UserId, 
            command.Name, 
            project.Id, 
            project.ChatFolders.FirstOrDefault(f => f.Id == parentId), 
            command.EmojiId);
        
        if (createFolderResult.IsFailure)
            return createFolderResult.Error;
        
        project.AddFolder(createFolderResult.Value);
        await _unitOfWork.SaveChanges(cancellationToken);

        return createFolderResult.Value.Id.Value;
    }
}