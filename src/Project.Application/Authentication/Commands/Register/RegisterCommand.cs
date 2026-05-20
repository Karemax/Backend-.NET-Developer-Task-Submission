using MediatR;
using Project.Application.Abstractions.Authentication;
using Project.Application.Abstractions.Data;
using Project.Application.Authentication.Common;
using Project.Domain.Entities;
using Project.Domain.Enums;
using Project.Domain.Errors;
using Project.Domain.Primitives;

namespace Project.Application.Authentication.Commands.Register;

public record RegisterCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName) : IRequest<Result<AuthResponse>>;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtProvider _jwtProvider;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtProvider jwtProvider,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtProvider = jwtProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AuthResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existingUser is not null)
        {
            return Result.Failure<AuthResponse>(DomainErrors.User.EmailAlreadyExists);
        }

        string passwordHash = _passwordHasher.Hash(request.Password);

        var user = User.Create(
            Guid.NewGuid(),
            request.Email,
            passwordHash,
            request.FirstName,
            request.LastName,
            Role.User);

        _userRepository.Add(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        string token = _jwtProvider.Generate(user);

        return new AuthResponse(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            token);
    }
}
