using CSharpFunctionalExtensions;
using NeuralForge.Domain.Chats;
using NeuralForge.Domain.Chats.ValueObjects.Ids;
using NeuralForge.Domain.Folders;
using NeuralForge.Domain.Folders.ValueObjects.Ids;
using NeuralForge.Domain.Fragments.ValueObjects.Ids;
using NeuralForge.Domain.Shared;
using NeuralForge.Domain.Shared.ValueObjects.Ids;

namespace NeuralForge.Domain.Fragments;

public class Fragment : Shared.Entity<FragmentId>
{
    public FolderId FolderId { get; }
    
    public string Name { get; }
    public string Content { get; }
    
    public ChatId ChatId { get; }
    public MessageId MessageId { get; }

    private Fragment(FragmentId id, string name, string content, ChatId chatId, MessageId messageId, FolderId folderId) : base(id)
    {
        Name = name;
        Content = content;
        ChatId = chatId;
        MessageId = messageId;
        FolderId = folderId;
    }

    public static Result<Fragment, ErrorList> Create(string name, string content, Chat chat, Message message, FolderId folderId)
    {
        var errors = new List<Error>();
        
        if (string.IsNullOrWhiteSpace(name))
            errors.Add(Errors.General.ValueIsInvalid("name"));
        
        if (string.IsNullOrWhiteSpace(content))
            errors.Add(Errors.General.ValueIsInvalid("content"));
        
        if (chat.Id != message.ChatId)
            errors.Add(Errors.General.ValueIsInvalid("chatId"));

        if (errors.Count > 0)
            return (ErrorList)errors;
        
        return new Fragment(FragmentId.AddNewId(), name, content, chat.Id, message.Id, folderId);
    }
}