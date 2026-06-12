using CSharpFunctionalExtensions;
using NeuralForge.Domain.Chats;
using NeuralForge.Domain.Folders.ValueObjects.Ids;
using NeuralForge.Domain.Shared;

namespace NeuralForge.Domain.Folders;

public class ChatFolder : Folder
{
    private List<Chat> _chats = [];
    public IReadOnlyList<Chat> Chats => _chats;
    
    private ChatFolder(FolderId id, FolderId? parentId, string name, int emojiId) : base(id, parentId, name, emojiId)
    {
    }
    
    public static Result<ChatFolder, ErrorList> Create(string name, FolderId? parentId = null, int? emojiId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            return (ErrorList)Errors.General.ValueIsRequired("name");
        
        if (emojiId < 0)
            return (ErrorList)Errors.General.ValueIsInvalid("emojiId");
        
        return new ChatFolder(FolderId.AddNewId(), parentId, name, emojiId ?? 0);
    }
}