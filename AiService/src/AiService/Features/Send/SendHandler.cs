using System.Text.Json;
using AiService.Abstractions;
using AiService.Db;
using AiService.Models.AiClients.Contracts;
using AiService.Models.AiClients.Dtos;
using AiService.Models.AiMapping;
using AiService.Models.Chats;
using AiService.Models.Chats.ValueObjects;
using AiService.Models.Chats.ValueObjects.Ids;
using AiService.Models.Shared;
using AiService.Providers;
using AiService.Services.Tokenizers;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Pgvector.EntityFrameworkCore;
using IResult = Microsoft.AspNetCore.Http.IResult;
using Message = AiService.Models.Chats.ValueObjects.Message;

namespace AiService.Features.Send;

public class SendHandler : ICommandHandler
{
    private readonly AppDbContext _dbContext;
    private readonly AiRegistry _aiRegistry;
    private readonly ITokenizerService _tokenizerService;
    private readonly AiProvider _aiProvider;
    private readonly ILogger<SendHandler> _logger;
    
    public SendHandler(
        AppDbContext dbContext, 
        AiRegistry aiRegistry, 
        ITokenizerService tokenizerService, 
        AiProvider aiProvider, 
        ILogger<SendHandler> logger)
    {
        _dbContext = dbContext;
        _aiRegistry = aiRegistry;
        _tokenizerService = tokenizerService;
        _aiProvider = aiProvider;
        _logger = logger;
    }

    public async Task<Result<string, Error>> Handle(
        SendCommand command,
        CancellationToken cancellationToken = default)
    {
        if (_aiRegistry.TryGet(command.Model, out var aiModelConfig) == false)
            return Error.Failure("model.not.found", $"Model {command.Model} not found");

        var tokenizerHandlerResult = _tokenizerService.GetTokenizerHandler(command.Model);
        
        if (tokenizerHandlerResult.IsFailure) 
            return tokenizerHandlerResult.Error;
        
        var tokenizerHandler = tokenizerHandlerResult.Value;
        var tokenCount = tokenizerHandler.GetTokenCount(command.Prompt);

        if (tokenCount > Constants.MAX_CLIENT_MESSAGE_TOKENS)
            return Error.Failure("too.many.tokens",
                $"Too many tokens: {tokenCount}. Max: {Constants.MAX_CLIENT_MESSAGE_TOKENS}");

        var chatResult = await _dbContext.Chats.FirstOrDefaultAsync(x => 
            x.UserId == command.UserId && x.Id == command.ChatId, cancellationToken);
        
        if (chatResult is null)
        {
            var newChatResult = Chat.Create(ChatId.AddNewId(), command.UserId);
            
            if (newChatResult.IsFailure) 
                return newChatResult.Error;
            
            chatResult = newChatResult.Value;
            _dbContext.Chats.Add(chatResult);
        }
        
        var promptEmbedding = await _aiProvider.GenerateEmbeddingAsync(
            [command.Prompt], 
            cancellationToken);
        
        var queryVector = new Pgvector.Vector(promptEmbedding[0]);
        
        var topMemories = await _dbContext.ChatMemories
            .Where(m => m.ChatId == chatResult.Id.Value)
            .OrderBy(m => m.Embedding.CosineDistance(queryVector)) 
            .Take(3) 
            .Select(m => m.Content)
            .ToListAsync(cancellationToken);
        
        var clientMessage = Message.Create(command.Prompt, Role.User);

        if (clientMessage.IsFailure)
            return clientMessage.Error;
        
        chatResult.UpdateLastMessages(tokenizerHandler.GetTokenCount, clientMessage.Value);

        var aiResponse = await _aiProvider.GenerateTextAsync(command.Model, BuildMemoryPayload(chatResult, topMemories), 0.7f,
            cancellationToken);

        var aiMessage = Message.Create(aiResponse.Content, Role.Assistant, command.Model);
        
        if (aiMessage.IsFailure)
            return aiMessage.Error;
        
        chatResult.UpdateLastMessages(tokenizerHandler.GetTokenCount, aiMessage.Value);
        
        if (chatResult.IsNeedToSummarize(tokenizerHandler.GetTokenCount))
            await OptimizeMemoryAsync(chatResult, _aiRegistry, _aiProvider, 
                tokenizerHandler.GetTokenCount, _dbContext, _logger, cancellationToken);

        chatResult.AddAiModel(command.Model);
        
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return aiResponse.Content;
    }

    private static List<ChatMessageDto> BuildMemoryPayload(Chat chat, List<string> relevantMemories)
    {
        var payload = new List<ChatMessageDto>
        {
            new("system", $"Ты полезный ИИ-ассистент с долгосрочной памятью. Твой ответ должен быть достаточно " +
                          $"лаконичным, чтобы уложиться в {Constants.MAX_AI_MESSAGE_TOKENS} токенов.")
        };
        
        if (string.IsNullOrEmpty(chat.Summary) == false)
        {
            payload.Add(new("system", $"Краткое содержание предыдущей беседы: {chat.Summary}"));
        }
        
        payload.AddRange(chat.LastMessages.Select(m => new ChatMessageDto(
            m.Role.ToString().ToLower(), 
            m.Value + (m.Model is not null ? $" (answered {m.Model})" : ""))));
        
        if (relevantMemories.Count > 0)
        {
            var facts = string.Join("\n", relevantMemories);
            payload.Add(new("system", $"Важные факты о контексте, которые ты должен помнить:\n{facts}"));
        }
        
        return payload;
    }
    
    private static async Task OptimizeMemoryAsync(
        Chat chat, 
        AiRegistry aiRegistry, 
        AiProvider aiProvider, 
        Func<string, int> tokenizerFunc,
        AppDbContext dbContext,
        ILogger<SendHandler> logger,
        CancellationToken ct)
    {
        var instructions = new List<ChatMessageDto>
        {
            new("system", "Ты — модуль оптимизации памяти чата. Прочитай текущее саммари и новые сообщения. " +
                          "Твоя задача:\n" +
                          "1. Сформировать обновленное, сжатое саммари всей беседы (сюжет).\n" +
                          "2. Выделить из новых сообщений важные атомарные факты о пользователе/проекте для " +
                          "долгосрочного архива.\n" +
                          "Верни ответ СТРОГО в формате JSON:\n" +
                          "{\n" +
                          "  \"newSummary\": \"текст обновленного саммари\",\n" +
                          "  \"memories\": [\"факт 1\", \"факт 2\"]\n" +
                          "}\n" +
                          "Если новых фактов нет, верни пустой массив []"),
            new("user", $"Текущее саммари: {chat.Summary}\n\nНовые сообщения:\n" + 
                        string.Join("\n", chat.MessagesForSummarize.Select(m => $"{m.Role}: {m.Value}")))
        };

        var summaryResponse = await aiProvider.GenerateTextAsync(
            aiRegistry.AiModelForSummarizationName, 
            instructions, 
            0.2f, 
            ct,
            Constants.MAX_MEMORIES_TOKENS + Constants.MAX_SUMMARY_TOKENS + 50);
        
        try
        {
            var result = JsonSerializer.Deserialize<MemoryOptimizationResult>(
                summaryResponse.Content, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (result is null)
            {
                logger.LogError("Ошибка при парсинге ответа на запрос оптимизации памяти");
                return;
            }
            
            var updateResult = chat.UpdateSummary(tokenizerFunc, result.NewSummary);
            
            if (updateResult.IsFailure)
                logger.LogError($"{updateResult.Error.Code}: {updateResult.Error.Message}");
            
            chat.CleanMessagesForSummarize();
            
            if (result.Memories.Count < 1)
                return;
            
            var embeddings = await aiProvider.GenerateEmbeddingAsync(result.Memories, ct);
            
            for (int i = 0; i < result.Memories.Count; i++)
            {
                var fact = result.Memories[i];
                var embedding = embeddings[i];

                var chatMemory = ChatMemory.Create(ChatMemoryId.AddNewId(), chat.Id, fact, embedding);

                if (chatMemory.IsFailure)
                {
                    logger.LogError($"{chatMemory.Error.Code}: {chatMemory.Error.Message}");
                    continue; 
                }
        
                dbContext.ChatMemories.Add(chatMemory.Value);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при парсинге или векторизации воспоминаний чата {ChatId}", chat.Id.Value);
        }
    }
}