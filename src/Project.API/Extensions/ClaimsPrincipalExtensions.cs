using System.Security.Claims;

namespace Project.API.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid? GetUserId(this ClaimsPrincipal user)
    {
        if (user is null)
        {
            return null;
        }

        var sub = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                  ?? user.FindFirst("sub")?.Value;

        return Guid.TryParse(sub, out var userId) ? userId : null;
    }
}
