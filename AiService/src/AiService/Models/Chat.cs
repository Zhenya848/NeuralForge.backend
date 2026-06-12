using System.Collections.Immutable;
using AiService.Models.Shared;
using AiService.Models.ValueObjects.Ids;
using CSharpFunctionalExtensions;

namespace AiService.Models;

public class Chat : Shared.Entity<ChatId>
{
    public string Name { get; }
    public string Summary { get; }
    private List<string> _memories { get; } = [];
    private Queue<Message> _lastMessages { get; } = [];
    public IReadOnlyList<string> Memories => _memories;
    
    private Chat(ChatId id, string name) : base(id)
    {
        Name = name;
    }

    public static Result<Chat, ErrorList> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return (ErrorList)Errors.General.ValueIsRequired("name");
        
        return new Chat(ChatId.AddNewId(), name);
    }

    public UnitResult<ErrorList> UpdateSummary(string summary)
    {
        if (string.IsNullOrWhiteSpace(summary))
            return (ErrorList)Errors.General.ValueIsRequired("summary");
        
        if (summary.Length > Constants.MAX_SUMMARY_LENGTH)
            return (ErrorList)Error.Validation("summary.too.long", 
                $"Summary length is too long. Max length: {Constants.MAX_SUMMARY_LENGTH}");

        return Result.Success<ErrorList>();
    }

    public UnitResult<ErrorList> UpdateMemories(List<string> memories)
    {
        if (memories.Any(string.IsNullOrWhiteSpace))
            return (ErrorList)Errors.General.ValueIsRequired("memory");
        
        if (memories.Sum(x => x.Length) > Constants.MAX_MEMORIES_LENGTH)
            return (ErrorList)Error.Validation("memories.too.long", 
                $"Summary length is too long. Max length: {Constants.MAX_MEMORIES_LENGTH}");
        
        return Result.Success<ErrorList>();
    }

    public List<Message> UpdateLastMessages(List<Message> messages)
    {
        var sumLength = _lastMessages.Sum(x => x.Value.Length);
        var messagesForSummorize = new List<Message>();

        messages = messages.OrderBy(x => x.SerialNumber).ToList();

        foreach (var m in messages.TakeWhile(m => m.Value.Length <= Constants.MAX_LAST_MESSAGES_LENGTH))
        {
            while (sumLength + m.Value.Length > Constants.MAX_LAST_MESSAGES_LENGTH)
            {
                var firstMessage = _lastMessages.Dequeue();
                
                messagesForSummorize.Add(firstMessage);
                sumLength -= firstMessage.Value.Length;
            }
            
            _lastMessages.Enqueue(m);
        }
        
        return messagesForSummorize;
    }
}