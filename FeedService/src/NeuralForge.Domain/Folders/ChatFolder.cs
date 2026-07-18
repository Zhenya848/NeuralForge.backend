using System.Text.Json.Serialization;
using CSharpFunctionalExtensions;
using NeuralForge.Domain.Chats;
using NeuralForge.Domain.Chats.ValueObjects.Ids;
using NeuralForge.Domain.Folders.ValueObjects.Ids;
using NeuralForge.Domain.Projects;
using NeuralForge.Domain.Projects.ValueObjects.Ids;
using NeuralForge.Domain.Shared;

namespace NeuralForge.Domain.Folders;

public class ChatFolder : Folder
{
    public ProjectId ProjectId { get; }
    private List<Chat> _chats = [];
    public IReadOnlyList<Chat> Chats => _chats;
    
    private ChatFolder(FolderId id) : base(id)
    {
        
    }
    
    private ChatFolder(FolderId id, Guid userId, FolderId? parentId, ProjectId projectId, string name, int emojiId) : base(id, userId, parentId, name, emojiId)
    {
        ProjectId = projectId;
    }
    
    public static Result<ChatFolder, ErrorList> Create(Guid userId, string name, ProjectId projectId, ChatFolder? parent = null, int? emojiId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            return (ErrorList)Errors.General.ValueIsRequired("name");
        
        if (emojiId < 0)
            return (ErrorList)Errors.General.ValueIsInvalid("emojiId");
        
        return new ChatFolder(FolderId.AddNewId(), userId, parent?.Id, projectId, name, emojiId ?? 0);
    }
    
    public void AddChat(Chat chat) => _chats.Add(chat);
}