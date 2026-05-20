using Project.Domain.Primitives;

namespace Project.Domain.Errors;

public static class DomainErrors
{
    public static class User
    {
        public static readonly Error EmailAlreadyExists = new(
            "User.EmailAlreadyExists",
            "The email is already in use.");

        public static readonly Error InvalidCredentials = new(
            "User.InvalidCredentials",
            "Invalid email or password.");
    }
}
