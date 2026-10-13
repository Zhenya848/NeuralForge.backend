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
    private readonly IProjectsRepository _projectsRepository;
    private readonly IChatFoldersRepository _chatFoldersRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateChatFolderHandler(
        IProjectsRepository projectsRepository, 
        IChatFoldersRepository chatFoldersRepository,
        IUnitOfWork unitOfWork)
    {
        _projectsRepository = projectsRepository;
        _chatFoldersRepository = chatFoldersRepository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Result<Guid, ErrorList>> Handle(
        CreateChatFolderCommand command, 
        CancellationToken cancellationToken = default)
    {
        ProjectId projectId;
        FolderId? parentFolderId = null;

        if (command.ProjectId is null)
        {
            projectId = await _projectsRepository.GetDefaultProjectId(
                command.UserId,
                cancellationToken);
        }
        else
        {
            var projectIdValue = ProjectId.Create(command.ProjectId.Value);

            var isProjectExist = await _projectsRepository.IsProjectExist(
                command.UserId,
                projectIdValue,
                cancellationToken);

            if (isProjectExist == false)
                return (ErrorList)Errors.General.NotFound(command.ProjectId.Value);

            projectId = projectIdValue;
        }

        if (command.ParentId is not null)
        {
            parentFolderId = FolderId.Create(command.ParentId.Value);

            var isFolderExist = await _chatFoldersRepository.IsFolderExist(
                command.UserId,
                projectId,
                parentFolderId,
                cancellationToken);

            if (!isFolderExist)
                return (ErrorList)Errors.General.NotFound(command.ParentId.Value);
        }

        var createFolderResult = ChatFolder.Create(
            command.UserId, 
            command.Name, 
            projectId, 
            parentFolderId, 
            command.EmojiId);
        
        if (createFolderResult.IsFailure)
            return createFolderResult.Error;
        
        _chatFoldersRepository.Create(createFolderResult.Value);
        await _unitOfWork.SaveChanges(cancellationToken);

        return createFolderResult.Value.Id.Value;
    }
}