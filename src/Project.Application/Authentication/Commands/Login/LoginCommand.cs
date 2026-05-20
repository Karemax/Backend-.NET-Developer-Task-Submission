using MediatR;
using Project.Application.Abstractions.Authentication;
using Project.Application.Abstractions.Data;
using Project.Application.Authentication.Common;
using Project.Domain.Errors;
using Project.Domain.Primitives;

namespace Project.Application.Authentication.Commands.Login;

public record LoginCommand(
    string Email,
    string Password) : IRequest<Result<AuthResponse>>;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtProvider _jwtProvider;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtProvider jwtProvider)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtProvider = jwtProvider;
    }

    public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user is null)
        {
            return Result.Failure<AuthResponse>(DomainErrors.User.InvalidCredentials);
        }

        bool verified = _passwordHasher.Verify(request.Password, user.PasswordHash);
        if (!verified)
        {
            return Result.Failure<AuthResponse>(DomainErrors.User.InvalidCredentials);
        }

        string token = _jwtProvider.Generate(user);

        return new AuthResponse(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            token);
    }
}
