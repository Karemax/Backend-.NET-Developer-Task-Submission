using MediatR;
using Project.Application.Abstractions.Caching;
using Project.Application.Abstractions.Data;
using Project.Domain.Primitives;
using FluentValidation;

namespace Project.Application.Projects.Commands.DeleteProject;

public record DeleteProjectCommand(Guid ProjectId, Guid UserId) : IRequest<Result>;

public class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand, Result>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;

    public DeleteProjectCommandHandler(IProjectRepository projectRepository, IUnitOfWork unitOfWork, ICacheService cacheService)
    {
        _projectRepository = projectRepository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<Result> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetByIdAsync(request.ProjectId, cancellationToken);
        if (project is null)
            return Result.Failure(Error.NullValue);

        if (project.OwnerId != request.UserId)
            return Result.Failure(new Error("Error.Unauthorized", "User is not authorized to delete this project."));

        _projectRepository.Remove(project);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _cacheService.RemoveAsync($"project:{request.ProjectId}", cancellationToken);
        return Result.Success();
    }
}

public class DeleteProjectCommandValidator : AbstractValidator<DeleteProjectCommand>
{
    public DeleteProjectCommandValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
    }
}
