using System.Security.Claims;

namespace BlazorWebAppOidc.Client;

// Add properties to this class and update the server and client AuthenticationStateProviders
// to expose more information about the authenticated user to the client.
public sealed class UserInfo
{
    public required string UserId { get; init; }
    public required string Name { get; init; }
    public IEnumerable<UserClaim> Claims { get; init; }

    public const string UserIdClaimType = "sub";
    public const string NameClaimType = "name";
    public const string RoleClaimType = "role";


    public static UserInfo FromClaimsPrincipal(ClaimsPrincipal principal)
    {
        var userInfo = new UserInfo() {
            UserId = GetRequiredClaim(principal, UserIdClaimType),
            Name = GetRequiredClaim(principal, NameClaimType),
            Claims = principal.Claims.Select(c => new UserClaim(c.Type, c.Value))
        };

        return userInfo;
    }

    public ClaimsPrincipal ToClaimsPrincipal()
    {
        var claimsIdentity = new ClaimsPrincipal(new ClaimsIdentity(
            Claims.Select(c => new Claim(type: c.ClaimType, value: c.ClaimValue)),
            authenticationType: nameof(UserInfo),
            nameType: NameClaimType,
            roleType: RoleClaimType));

        return claimsIdentity;
    }

    private static string GetRequiredClaim(ClaimsPrincipal principal, string claimType) =>
        principal.FindFirst(claimType)?.Value ?? throw new InvalidOperationException($"Could not find required '{claimType}' claim.");
}

public sealed class UserClaim(string claimType, string claimValue)
{
    public string ClaimType { get; init; } = claimType;
    public string ClaimValue { get; init; } = claimValue;
}
