using CSharpFunctionalExtensions;
using NeuralForge.Application.Abstractions;
using NeuralForge.Application.Repositories;
using NeuralForge.Domain.Chats.ValueObjects;
using NeuralForge.Domain.Chats.ValueObjects.Ids;
using NeuralForge.Domain.Folders.ValueObjects.Ids;
using NeuralForge.Domain.Projects.ValueObjects.Ids;
using NeuralForge.Domain.Shared;

namespace NeuralForge.Application.Messages.Send;

public class SendMessageHandler : ICommandHandler<SendMessageCommand, Result<Guid, ErrorList>>
{
    private readonly IChatsRepository _repository;
    private readonly IGreeterService _greeterService;
    private readonly IUnitOfWork _unitOfWork;

    public SendMessageHandler(IChatsRepository repository, IGreeterService greeterService, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _greeterService = greeterService;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Result<Guid, ErrorList>> Handle(
        SendMessageCommand command, 
        CancellationToken cancellationToken = default)
    {
        var chatResult = await _repository.GetById(
            command.UserId,
            ChatId.Create(command.ChatId),
            cancellationToken);
        
        if (chatResult.IsFailure)
            return chatResult.Error;
        
        var chat = chatResult.Value;
        var addClientMessageResult = chat.AddClientMessage(command.Prompt);
        
        if (addClientMessageResult.IsFailure)
            return addClientMessageResult.Error;

        var result = await _greeterService
            .SendToAi(command.UserId, command.ChatId, command.Prompt, command.Model, cancellationToken);
        
        if (result.IsFailure)
            return result.Error;

        var aiModelResult = AiModel.Create(command.Model);
        
        if (aiModelResult.IsFailure)
            return aiModelResult.Error;
        
        var addAiMessageResult = chat.AddAiMessage(result.Value, aiModelResult.Value);
        
        if (addAiMessageResult.IsFailure)
            return addAiMessageResult.Error;

        await _unitOfWork.SaveChanges(cancellationToken);

        return chat.Id.Value;
    }
}