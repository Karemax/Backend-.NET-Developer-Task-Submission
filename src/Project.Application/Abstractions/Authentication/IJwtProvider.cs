using Project.Domain.Entities;

namespace Project.Application.Abstractions.Authentication;

public interface IJwtProvider
{
    string Generate(User user);
}
