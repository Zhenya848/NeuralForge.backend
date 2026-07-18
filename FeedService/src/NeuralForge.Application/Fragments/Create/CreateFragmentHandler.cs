using CSharpFunctionalExtensions;
using Dapper;
using Microsoft.Extensions.Logging;
using NeuralForge.Application.Abstractions;
using NeuralForge.Application.Repositories;
using NeuralForge.Domain.Chats;
using NeuralForge.Domain.Chats.Messages;
using NeuralForge.Domain.Chats.ValueObjects.Ids;
using NeuralForge.Domain.Folders;
using NeuralForge.Domain.Folders.ValueObjects.Ids;
using NeuralForge.Domain.Fragments;
using NeuralForge.Domain.Shared;
using NeuralForge.Domain.Shared.ValueObjects.Dtos;

namespace NeuralForge.Application.Fragments.Create;

public class CreateFragmentHandler : ICommandHandler<CreateFragmentCommand, Result<Guid, ErrorList>>
{
    private readonly IFragmentFoldersRepository _repository;
    private readonly ISqlConnectionFactory _sqlConnectionFactory;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateFragmentHandler> _logger;

    public CreateFragmentHandler(
        IFragmentFoldersRepository repository, 
        ISqlConnectionFactory sqlConnectionFactory, 
        IUnitOfWork unitOfWork,
        ILogger<CreateFragmentHandler> logger)
    {
        _repository = repository;
        _sqlConnectionFactory = sqlConnectionFactory;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    
    public async Task<Result<Guid, ErrorList>> Handle(
        CreateFragmentCommand command, 
        CancellationToken cancellationToken = default)
    {
        FragmentFolder folder;

        if (command.FolderId is not null)
        {
            var folderResult = await _repository.GetById(
                command.UserId, 
                FolderId.Create(command.FolderId.Value), 
                cancellationToken);

            if (folderResult.IsFailure)
                return folderResult.Error;
            
            folder = folderResult.Value;
        }
        else
            folder = await _repository.GetDefaultFolder(command.UserId, cancellationToken);

        try
        {
            using var connection = _sqlConnectionFactory.Create();

            var sqlMessageResult = """SELECT * FROM Messages m WHERE m.id = @id TAKE 1""";
            var messageResult = await connection
                .QuerySingleAsync<MessageDto>(sqlMessageResult, new { id = command.MessageId });
            
            var message = ClientMessage.Create(
                ChatId.Create(messageResult.ChatId),
                messageResult.SerialNumber,
                messageResult.Value).Value;
            
            var fragmentResult = Fragment.Create(command.UserId, command.Name, command.Content, message, folder);
        
            if (fragmentResult.IsFailure)
                return fragmentResult.Error;
        
            folder.AddFragment(fragmentResult.Value);
            await _unitOfWork.SaveChanges(cancellationToken);
        
            return fragmentResult.Value.Id.Value;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return (ErrorList)Errors.General.Failure();
        }
    }
}