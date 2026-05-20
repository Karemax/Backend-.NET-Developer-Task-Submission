using MediatR;
using Project.Application.Abstractions.Caching;
using Project.Application.Abstractions.Data;
using Project.Domain.Entities;
using Project.Domain.Primitives;

namespace Project.Application.Projects.Commands.UpdateProject;

public record UpdateProjectCommand(Guid Id, string Name, string Description, Guid UserId) : IRequest<Result>;

public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, Result>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;

    public UpdateProjectCommandHandler(IProjectRepository projectRepository, IUnitOfWork unitOfWork, ICacheService cacheService)
    {
        _projectRepository = projectRepository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<Result> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetByIdAsync(request.Id, cancellationToken);
        if (project is null)
            return Result.Failure(Error.NullValue);

        if (project.OwnerId != request.UserId)
            return Result.Failure(new Error("Error.Unauthorized", "User is not authorized to update this project."));

        project.Update(request.Name, request.Description);
        _projectRepository.Update(project);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _cacheService.RemoveAsync($"project:{request.Id}", cancellationToken);
        return Result.Success();
    }
}
