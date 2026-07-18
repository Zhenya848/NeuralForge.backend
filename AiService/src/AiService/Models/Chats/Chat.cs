using System.Text.Json.Serialization;
using AiService.Models.Chats.ValueObjects;
using AiService.Models.Chats.ValueObjects.Ids;
using AiService.Models.Shared;
using CSharpFunctionalExtensions;

namespace AiService.Models.Chats;

public class Chat : Shared.Entity<ChatId>
{
    public Guid UserId { get; }
    public string Summary { get; private set; } = "";
    private List<Message> _messagesForSummarize = [];
    public IReadOnlyList<Message> MessagesForSummarize => _messagesForSummarize;
    private Queue<Message> _lastMessages = [];
    public IReadOnlyCollection<Message> LastMessages => _lastMessages;
    private List<string> _aiModels = [];
    public IReadOnlyList<string> AiModels => _aiModels;

    [JsonConstructor]
    private Chat(ChatId id) : base(id)
    {
        
    }
    
    private Chat(ChatId id, Guid userId) : base(id)
    {
        UserId = userId;
    }

    public static Result<Chat, Error> Create(ChatId id, Guid userId)
    {
        if (userId.Equals(Guid.Empty))
            return Errors.General.ValueIsRequired("userId");
        
        return new Chat(id, userId);
    }
    
    public UnitResult<Error> UpdateSummary(Func<string, int> tokenCounter, string summary)
    {
        if (string.IsNullOrWhiteSpace(summary))
            return Errors.General.ValueIsRequired("summary");
        
        if (tokenCounter(summary) > Constants.MAX_SUMMARY_TOKENS)
            return Error.Validation("summary.too.long", 
                $"Summary length is too long. Max length: {Constants.MAX_SUMMARY_TOKENS}");
        
        Summary = summary;

        return Result.Success<Error>();
    }
    
    public void CleanMessagesForSummarize() => _messagesForSummarize.Clear();
    public bool IsNeedToSummarize(Func<string, int> tokenCounter) => 
        _messagesForSummarize.Sum(x => tokenCounter(x.Value)) >= Constants.MAX_SUMMARY_TOKENS;

    public void UpdateLastMessages(Func<string, int> tokenCounter, params List<Message> messages)
    {
        var sumTokens = _lastMessages.Sum(x => tokenCounter(x.Value));
        var orderedMessages = messages.OrderBy(x => x.SerialNumber).ToList();

        foreach (var m in orderedMessages)
        {
            var messageTokens = tokenCounter(m.Value);

            if (messageTokens > Constants.MAX_LAST_MESSAGES_TOKENS)
                continue;
            
            while (sumTokens + messageTokens > Constants.MAX_LAST_MESSAGES_TOKENS)
            {
                if (_lastMessages.TryDequeue(out var firstMessage))
                {
                    _messagesForSummarize.Add(firstMessage);
                    sumTokens -= tokenCounter(firstMessage.Value);
                }
                else
                    break;
            }
            
            m.ChangeSerialNumber(_lastMessages.Count > 0 ? _lastMessages.Last().SerialNumber + 1 : 1);
            
            _lastMessages.Enqueue(m);
            sumTokens += messageTokens; 
        }
    }

    public UnitResult<Error> AddAiModel(string model)
    {
        if (string.IsNullOrWhiteSpace(model))
            return Errors.General.ValueIsRequired("ai model");
        
        if (_aiModels.Contains(model) == false)
            _aiModels.Add(model);
            
        return UnitResult.Success<Error>();
    }
}