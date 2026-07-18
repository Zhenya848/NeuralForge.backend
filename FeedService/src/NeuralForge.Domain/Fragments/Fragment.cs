using System.Text.Json.Serialization;
using CSharpFunctionalExtensions;
using NeuralForge.Domain.Chats;
using NeuralForge.Domain.Chats.Messages;
using NeuralForge.Domain.Chats.ValueObjects.Ids;
using NeuralForge.Domain.Folders;
using NeuralForge.Domain.Folders.ValueObjects.Ids;
using NeuralForge.Domain.Fragments.ValueObjects.Ids;
using NeuralForge.Domain.Shared;
using NeuralForge.Domain.Shared.ValueObjects.Ids;

namespace NeuralForge.Domain.Fragments;

public class Fragment : Shared.Entity<FragmentId>
{
    public Guid UserId { get; }
    public FolderId FolderId { get; private set; }
    
    public string Name { get; private set; }
    public string Content { get; }
    
    public Message Message { get; }

    [JsonConstructor]
    private Fragment(FragmentId id) : base(id)
    {
        
    }
    
    private Fragment(FragmentId id, Guid userId, string name, string content, Message message, FolderId folderId) : base(id)
    {
        UserId = userId;
        Name = name;
        Content = content;
        Message = message;
        FolderId = folderId;
    }

    public static Result<Fragment, ErrorList> Create(Guid userId, string name, string content, Message message, FragmentFolder folder)
    {
        var errors = new List<Error>();
        
        if (string.IsNullOrWhiteSpace(name))
            errors.Add(Errors.General.ValueIsInvalid("name"));
        
        if (string.IsNullOrWhiteSpace(content))
            errors.Add(Errors.General.ValueIsInvalid("content"));

        if (errors.Count > 0)
            return (ErrorList)errors;
        
        return new Fragment(FragmentId.AddNewId(), userId, name, content, message, folder.Id);
    }

    public UnitResult<ErrorList> Update(string name, FragmentFolder? folder = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            return (ErrorList)Errors.General.ValueIsInvalid("name");
        
        Name = name.Trim();
        FolderId = folder?.Id ?? FolderId;

        return Result.Success<ErrorList>();
    }
}