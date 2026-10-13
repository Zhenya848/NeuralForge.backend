using System.Text.Json.Serialization;
using CSharpFunctionalExtensions;
using NeuralForge.Domain.Chats;
using NeuralForge.Domain.Folders;
using NeuralForge.Domain.Projects.ValueObjects.Ids;
using NeuralForge.Domain.Shared;

namespace NeuralForge.Domain.Projects;

public class Project : Shared.Entity<ProjectId>
{
    public Guid UserId { get; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public int EmojiId { get; private set; }
    public int ColorId { get; private set; }
    public bool IsSystem { get; init; } = false;
    private List<FragmentFolder> _fragmentFolders = [];
    public IReadOnlyList<FragmentFolder> FragmentFolders => _fragmentFolders;
    private List<ChatFolder> _chatFolders = [];
    public IReadOnlyList<ChatFolder> ChatFolders => _chatFolders;
    
    [JsonConstructor]
    protected Project(ProjectId id) : base(id)
    {
        
    }

    protected Project(ProjectId id, Guid userId, string name, string description, int emojiId, int colorId, bool isSystem) : base(id)
    {
        UserId = userId;
        Name = name;
        Description = description;
        EmojiId = emojiId;
        ColorId = colorId;
    }
    
    public static Result<Project, ErrorList> Create(Guid userId, string name, string description, int emojiId, int colorId, bool isSystem = false)
    {
        var errors = new List<Error>();
        
        if (userId.Equals(Guid.Empty))
            errors.Add(Errors.General.ValueIsRequired("user id"));
        
        if (string.IsNullOrWhiteSpace(name))
            errors.Add(Errors.General.ValueIsRequired("name"));
        
        if (emojiId < 0)
            errors.Add(Errors.General.ValueIsInvalid("emoji id"));
        
        if (colorId < 0)
            errors.Add(Errors.General.ValueIsInvalid("color id"));
        
        if (errors.Count > 0)
            return (ErrorList)errors;
        
        return new Project(ProjectId.AddNewId(), userId, name, description, emojiId, colorId, isSystem);
    }

    public void AddFolder(Folder folder)
    {
        switch (folder)
        {
            case FragmentFolder fragmentFolder:
                _fragmentFolders.Add(fragmentFolder);
                break;
            case ChatFolder chatFolder:
                _chatFolders.Add(chatFolder);
                break;
        }
    }

    public UnitResult<ErrorList> Update(string name, string description, int emojiId, int colorId)
    {
        var errors = new List<Error>();
        
        if (string.IsNullOrWhiteSpace(name))
            errors.Add(Errors.General.ValueIsRequired("name"));
        
        if (string.IsNullOrWhiteSpace(description))
            errors.Add(Errors.General.ValueIsRequired("description"));
        
        if (emojiId < 0)
            errors.Add(Errors.General.ValueIsInvalid("emoji id"));
        
        if (colorId < 0)
            errors.Add(Errors.General.ValueIsInvalid("color id"));
        
        if (errors.Count > 0)
            return (ErrorList)errors;

        Name = name;
        Description = description;
        EmojiId = emojiId;
        ColorId = colorId;
        
        return Result.Success<ErrorList>();
    }
}