using CSharpFunctionalExtensions;
using NeuralForge.Domain.Folders.ValueObjects.Ids;
using NeuralForge.Domain.Fragments;
using NeuralForge.Domain.Shared;

namespace NeuralForge.Domain.Folders;

public class FragmentFolder : Folder
{
    private List<Fragment> _fragments = [];
    public IReadOnlyList<Fragment> Fragments => _fragments;
    
    private FragmentFolder(FolderId id, FolderId? parentId, string name, int emojiId) : base(id, parentId, name, emojiId)
    {
    }
    
    public static Result<FragmentFolder, ErrorList> Create(string name, FolderId? parentId = null, int? emojiId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            return (ErrorList)Errors.General.ValueIsRequired("name");
        
        if (emojiId < 0)
            return (ErrorList)Errors.General.ValueIsInvalid("emojiId");
        
        return new FragmentFolder(FolderId.AddNewId(), parentId, name, emojiId ?? 0);
    }
}