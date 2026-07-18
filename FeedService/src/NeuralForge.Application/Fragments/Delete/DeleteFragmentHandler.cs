using CSharpFunctionalExtensions;
using NeuralForge.Application.Abstractions;
using NeuralForge.Application.Repositories;
using NeuralForge.Domain.Folders.ValueObjects.Ids;
using NeuralForge.Domain.Fragments.ValueObjects.Ids;
using NeuralForge.Domain.Shared;

namespace NeuralForge.Application.Fragments.Delete;

public class DeleteFragmentHandler : ICommandHandler<DeleteFragmentCommand, Result<Guid, ErrorList>>
{
    private readonly IFragmentsRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteFragmentHandler(IFragmentsRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Result<Guid, ErrorList>> Handle(
        DeleteFragmentCommand command, 
        CancellationToken cancellationToken = default)
    {
        var fragmentResult = await _repository
            .GetById(command.UserId, FragmentId.Create(command.FragmentId), cancellationToken);
        
        if (fragmentResult.IsFailure)
            return fragmentResult.Error;

        var result = _repository.Delete(fragmentResult.Value);
        await _unitOfWork.SaveChanges(cancellationToken);
        
        return result;
    }
}