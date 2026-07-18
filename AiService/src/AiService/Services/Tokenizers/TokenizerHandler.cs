using Tokenizers.HuggingFace.Tokenizer;

namespace AiService.Services.Tokenizers;

public class TokenizerHandler
{
    private readonly Tokenizer _tokenizer;

    public TokenizerHandler(Tokenizer tokenizer)
    {
        _tokenizer = tokenizer;
    }

    public int GetTokenCount(string text)
    {
        var result = _tokenizer.Encode(text, addSpecialTokens: true).FirstOrDefault();

        return result?.Ids.Count ?? text.Length / 3;
    }
}