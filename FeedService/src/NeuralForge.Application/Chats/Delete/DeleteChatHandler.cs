using CSharpFunctionalExtensions;
using NeuralForge.Application.Abstractions;
using NeuralForge.Application.Fragments.Delete;
using NeuralForge.Application.Repositories;
using NeuralForge.Domain.Chats.ValueObjects.Ids;
using NeuralForge.Domain.Fragments.ValueObjects.Ids;
using NeuralForge.Domain.Shared;

namespace NeuralForge.Application.Chats.Delete;

public class DeleteChatHandler : ICommandHandler<DeleteChatCommand, Result<Guid, ErrorList>>
{
    private readonly IChatsRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteChatHandler(IChatsRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Result<Guid, ErrorList>> Handle(
        DeleteChatCommand command, 
        CancellationToken cancellationToken = default)
    {
        var chatResult = await _repository
            .GetById(command.UserId, ChatId.Create(command.ChatId), cancellationToken);
        
        if (chatResult.IsFailure)
            return chatResult.Error;

        var result = _repository.Delete(chatResult.Value);
        await _unitOfWork.SaveChanges(cancellationToken);
        
        return result;
    }
}