using System.Text.Json.Serialization;
using AiService.Models.Chats.ValueObjects.Ids;
using AiService.Models.Shared;
using CSharpFunctionalExtensions;
using Pgvector;

namespace AiService.Models.Chats;

public class ChatMemory : Shared.Entity<ChatMemoryId>
{
    public ChatId ChatId { get; private set; }
    public string Content { get; private set; }
    
    public Vector Embedding { get; private set; }
    
    private ChatMemory(ChatMemoryId id) : base(id) { } 

    private ChatMemory(ChatMemoryId id, ChatId chatId, string content, float[] embeddingArray) : base(id)
    {
        ChatId = chatId;
        Content = content;
        Embedding = new Vector(embeddingArray);
    }

    public static Result<ChatMemory, Error> Create(ChatMemoryId id, ChatId chatId, string content, float[] embeddingArray)
    {
        if (string.IsNullOrWhiteSpace(content))
            return Errors.General.ValueIsRequired("content");
        
        if (embeddingArray.Length == 0)
            return Errors.General.ValueIsRequired("embedding array");
        
        return new ChatMemory(id, chatId, content, embeddingArray);
    }
}