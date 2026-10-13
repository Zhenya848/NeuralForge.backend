using System.Text.Json.Serialization;
using CSharpFunctionalExtensions;
using NeuralForge.Domain.Chats.Messages;
using NeuralForge.Domain.Chats.ValueObjects;
using NeuralForge.Domain.Chats.ValueObjects.Ids;
using NeuralForge.Domain.Folders;
using NeuralForge.Domain.Folders.ValueObjects.Ids;
using NeuralForge.Domain.Shared;

namespace NeuralForge.Domain.Chats;

public class Chat : Shared.Entity<ChatId>
{
    public Guid UserId { get; }
    public FolderId FolderId { get; private set; }
    public string Name { get; private set; }
    
    private List<Message> _messages = [];
    public IReadOnlyList<Message> Messages => _messages;
    private List<ChatId> _connections = [];
    public IReadOnlyList<ChatId> Connections => _connections;

    public IEnumerable<string> GetUsedModels()
    {
        return _messages.Where(x => x is AiMessage)
            .Select(x => (x as AiMessage)!.AiModel.Name).Distinct();
    }

    [JsonConstructor]
    private Chat(ChatId id) : base(id)
    {
        
    }
    
    private Chat(ChatId id, Guid userId, string name, FolderId folderId) 
        : base(id)
    {
        Name = name;
        FolderId = folderId;
        UserId = userId;
    }

    public static Result<Chat, ErrorList> Create(
        Guid userId,
        string name, 
        FolderId folderId)
    {
        if (string.IsNullOrWhiteSpace(name))
            return (ErrorList)Errors.General.ValueIsInvalid("name");
        
        return new Chat(ChatId.AddNewId(), userId, name, folderId);
    }

    public void AddConnection(Chat chat)
    {
        AddConnectionInternal(chat.Id);
        chat.AddConnectionInternal(Id);
    }

    public void DeleteConnection(Chat chat)
    {
        RemoveConnectionInternal(chat.Id);
        chat.RemoveConnectionInternal(Id);
    }
    
    private void RemoveConnectionInternal(ChatId id)
    {
        _connections.Remove(id);
    }
    
    private void AddConnectionInternal(ChatId id)
    {
        if (_connections.Contains(id) == false)
            _connections.Add(id);
    }

    public UnitResult<ErrorList> AddClientMessage(string value)
    {
        var createResult = ClientMessage.Create(Id, _messages.Count + 1, value);
        
        if (createResult.IsFailure)
            return createResult.Error;
        
        _messages.Add(createResult.Value);
        return Result.Success<ErrorList>();
    }

    public UnitResult<ErrorList> AddAiMessage(string value, AiModel model)
    {
        var createResult = AiMessage.Create(Id, _messages.Count + 1, value, model);

        if (createResult.IsFailure)
            return createResult.Error;
        
        _messages.Add(createResult.Value);
        return Result.Success<ErrorList>();
    }

    public UnitResult<ErrorList> Update(string name, FolderId? folderId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            return (ErrorList)Errors.General.ValueIsRequired("name");
        
        Name = name;
        FolderId = folderId ?? FolderId;
        
        return Result.Success<ErrorList>();
    }
}