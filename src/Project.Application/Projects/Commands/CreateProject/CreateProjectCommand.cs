using MediatR;
using Project.Application.Abstractions.Data;
using Project.Application.Abstractions.Authentication;
using Project.Application.Authentication.Common;
using Project.Domain.Entities;
using Project.Domain.Primitives;
using Project.Domain.Errors;
using System;

namespace Project.Application.Projects.Commands.CreateProject;

public record CreateProjectCommand(string Name, string Description, Guid OwnerId) : IRequest<Result<Guid>>;

public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, Result<Guid>>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;
    // private readonly IJwtProvider _jwtProvider;

    public CreateProjectCommandHandler(IProjectRepository projectRepository, IUnitOfWork unitOfWork)
    {
        _projectRepository = projectRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = Domain.Entities.Project.Create(Guid.NewGuid(), request.Name, request.Description, request.OwnerId);
        _projectRepository.Add(project);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(project.Id);
    }
}
