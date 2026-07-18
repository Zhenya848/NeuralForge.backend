using AiService.Models.Shared;
using CSharpFunctionalExtensions;

namespace AiService.Services.Tokenizers;

public interface ITokenizerService
{
    Result<TokenizerHandler, Error> GetTokenizerHandler(string modelName);
}