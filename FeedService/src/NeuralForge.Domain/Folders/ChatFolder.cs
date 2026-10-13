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
    public ProjectId ProjectId { get; private set; }
    private List<Chat> _chats = [];
    public IReadOnlyList<Chat> Chats => _chats;
    
    private ChatFolder(FolderId id) : base(id)
    {
        
    }
    
    private ChatFolder(FolderId id, Guid userId, FolderId? parentId, ProjectId projectId, string name, int emojiId, bool isSystem) 
        : base(id, userId, parentId, name, emojiId, isSystem)
    {
        ProjectId = projectId;
    }
    
    public static Result<ChatFolder, ErrorList> Create(
        Guid userId, 
        string name, 
        ProjectId projectId, 
        FolderId? parentId = null, 
        int emojiId = 0,
        bool isSystem = false)
    {
        if (string.IsNullOrWhiteSpace(name))
            return (ErrorList)Errors.General.ValueIsRequired("name");
        
        if (emojiId < 0)
            return (ErrorList)Errors.General.ValueIsInvalid("emojiId");
        
        return new ChatFolder(FolderId.AddNewId(), userId, parentId, projectId, name, emojiId, isSystem);
    }
    
    public void AddChat(Chat chat) => _chats.Add(chat);
    public void ChangeProjectId(ProjectId projectId) => ProjectId = projectId;
}