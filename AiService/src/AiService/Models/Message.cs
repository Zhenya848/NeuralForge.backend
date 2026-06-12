using AiService.Models.Shared;
using AiService.Models.ValueObjects;
using AiService.Models.ValueObjects.Ids;
using CSharpFunctionalExtensions;

namespace AiService.Models;

public class Message : Shared.Entity<MessageId>
{
    public ChatId ChatId { get; }
    public int SerialNumber { get; }
    public string Value { get; }
    public Role Role { get; }
    
    private Message(MessageId id, ChatId chatId,int serialNumber, string value, Role role) : base(id)
    {
        ChatId = chatId;
        SerialNumber = serialNumber;
        Value = value;
        Role = role;
    }

    public static Result<Message, ErrorList> Create(ChatId chatId, int serialNumber, string value, Role role)
    {
        if (string.IsNullOrWhiteSpace(value))
            return (ErrorList)Errors.General.ValueIsRequired("value");
        
        if (serialNumber < 1)
            return (ErrorList)Errors.General.ValueIsInvalid("serialNumber");
        
        return new Message(MessageId.AddNewId(), chatId, serialNumber, value, role);
    }
}