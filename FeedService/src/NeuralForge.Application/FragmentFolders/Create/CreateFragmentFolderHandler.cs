using CSharpFunctionalExtensions;
using NeuralForge.Application.Abstractions;
using NeuralForge.Application.Repositories;
using NeuralForge.Domain.Folders;
using NeuralForge.Domain.Folders.ValueObjects.Ids;
using NeuralForge.Domain.Projects;
using NeuralForge.Domain.Projects.ValueObjects.Ids;
using NeuralForge.Domain.Shared;

namespace NeuralForge.Application.FragmentFolders.Create;

public class CreateFragmentFolderHandler : ICommandHandler<CreateFragmentFolderCommand, Result<Guid, ErrorList>>
{
    private readonly IProjectsRepository _projectsRepository;
    private readonly IFragmentFoldersRepository _fragmentFoldersRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateFragmentFolderHandler(
        IProjectsRepository projectsRepository, 
        IFragmentFoldersRepository fragmentFoldersRepository,
        IUnitOfWork unitOfWork)
    {
        _projectsRepository = projectsRepository;
        _fragmentFoldersRepository = fragmentFoldersRepository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Result<Guid, ErrorList>> Handle(
        CreateFragmentFolderCommand command, 
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

            var isFolderExist = await _fragmentFoldersRepository.IsFolderExist(
                command.UserId,
                projectId,
                parentFolderId,
                cancellationToken);

            if (!isFolderExist)
                return (ErrorList)Errors.General.NotFound(command.ParentId.Value);
        }
        
        var createFolderResult = FragmentFolder.Create(
            command.UserId,
            command.Name,
            projectId,
            parentFolderId,
            command.EmojiId);
        
        if (createFolderResult.IsFailure)
            return createFolderResult.Error;

        _fragmentFoldersRepository.Add(createFolderResult.Value);
        await _unitOfWork.SaveChanges(cancellationToken);

        return createFolderResult.Value.Id.Value;
    }
}