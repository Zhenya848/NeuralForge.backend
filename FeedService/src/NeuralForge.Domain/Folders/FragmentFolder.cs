using System.Text.Json.Serialization;
using CSharpFunctionalExtensions;
using NeuralForge.Domain.Folders.ValueObjects.Ids;
using NeuralForge.Domain.Fragments;
using NeuralForge.Domain.Fragments.ValueObjects.Ids;
using NeuralForge.Domain.Projects;
using NeuralForge.Domain.Projects.ValueObjects.Ids;
using NeuralForge.Domain.Shared;

namespace NeuralForge.Domain.Folders;

public class FragmentFolder : Folder
{
    public ProjectId ProjectId { get; }
    private List<Fragment> _fragments = [];
    public IReadOnlyList<Fragment> Fragments => _fragments;
    
    private FragmentFolder(FolderId id) : base(id)
    {
        
    }
    
    private FragmentFolder(FolderId id, Guid userId, FolderId? parentId, ProjectId projectId, string name, int emojiId, bool isSystem) 
        : base(id, userId, parentId, name, emojiId, isSystem)
    {
        ProjectId = projectId;
    }
    
    public static Result<FragmentFolder, ErrorList> Create(
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
        
        return new FragmentFolder(FolderId.AddNewId(), userId, parentId, projectId, name, emojiId, isSystem);
    }
    
    public void AddFragment(Fragment fragment) => _fragments.Add(fragment);
    public void RemoveFragment(Fragment fragment) => _fragments.Remove(fragment);
}