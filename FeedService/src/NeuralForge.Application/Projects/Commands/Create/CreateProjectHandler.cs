using CSharpFunctionalExtensions;
using NeuralForge.Application.Abstractions;
using NeuralForge.Application.Repositories;
using NeuralForge.Domain.Folders;
using NeuralForge.Domain.Projects;
using NeuralForge.Domain.Shared;

namespace NeuralForge.Application.Projects.Commands.Create;

public class CreateProjectHandler : ICommandHandler<CreateProjectCommand, Result<Guid, ErrorList>>
{
    private readonly IProjectsRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProjectHandler(IProjectsRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Result<Guid, ErrorList>> Handle(
        CreateProjectCommand command, 
        CancellationToken cancellationToken = default)
    {
        var createProjectResult = Project
            .Create(command.UserId, command.Name, command.Description, command.EmojiId, command.ColorId);

        if (createProjectResult.IsFailure)
            return createProjectResult.Error;

        var systemChatFolder = ChatFolder.Create(
            command.UserId, Constants.DefaultFolderName, createProjectResult.Value.Id, null, 0, true)
            .Value;

        var systemFragmentFolder = FragmentFolder.Create(
            command.UserId, Constants.DefaultFolderName, createProjectResult.Value.Id, null, 0, true)
            .Value;
        
        createProjectResult.Value.AddFolder(systemChatFolder);
        createProjectResult.Value.AddFolder(systemFragmentFolder);

        var result = _repository.Create(createProjectResult.Value);
        await _unitOfWork.SaveChanges(cancellationToken);
        
        return result;
    }
}