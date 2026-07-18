using AiService.Models.Shared;
using CSharpFunctionalExtensions;
using Tokenizers.HuggingFace.Tokenizer;

namespace AiService.Services.Tokenizers;

public class TokenizerService : ITokenizerService
{
    private readonly Dictionary<string, Tokenizer> _tokenizers = new(StringComparer.OrdinalIgnoreCase);
    private readonly ILogger<TokenizerService> _logger;

    public TokenizerService(ILogger<TokenizerService> logger)
    {
        _logger = logger;
        InitializeTokenizers();
    }
    
    private void InitializeTokenizers()
    {
        var tokenizersFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Tokenizers");

        if (!Directory.Exists(tokenizersFolder))
        {
            Directory.CreateDirectory(tokenizersFolder);
            return;
        }

        var jsonFiles = Directory.GetFiles(tokenizersFolder, "*.json");

        foreach (var filePath in jsonFiles)
        {
            try
            {
                var modelKey = Path.GetFileNameWithoutExtension(filePath).ToLower();
                
                var tokenizer = Tokenizer.FromFile(filePath);
                
                _tokenizers[modelKey] = tokenizer;
                _logger.LogInformation("🚀 Токенизатор для '{ModelKey}' успешно загружен.", modelKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Ошибка при загрузке токенизатора из файла: {FilePath}", filePath);
            }
        }
    }

    public Result<TokenizerHandler, Error> GetTokenizerHandler(string modelName)
    {
        modelName = modelName.ToLower();
        var tokenizerKey = _tokenizers.Keys.FirstOrDefault(x => x.Contains(modelName) || modelName.Contains(x));

        if (tokenizerKey is null)
            return Errors.General.NotFound();
        
        var tokenizer = _tokenizers[tokenizerKey];
        return new TokenizerHandler(tokenizer);
    }
}