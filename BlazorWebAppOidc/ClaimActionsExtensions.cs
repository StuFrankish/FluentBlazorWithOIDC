using Client.DomainElements;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;

namespace Client.Extensions;

public static class ClaimActionsExtensions
{
    public static void ApplyCustomClaimsActions(this ClaimActionCollection claimActions)
    {
        claimActions.Clear();
        claimActions.RemoveUnwantedClaimActions();
        claimActions.AddCustomClaimActions();
    }

    private static void AddCustomClaimActions(this ClaimActionCollection claimActions)
    {
        var addClaimActions = new List<string> {
            JwtClaimTypes.Name,
            JwtClaimTypes.Email,
            JwtClaimTypes.EmailVerified,
            JwtClaimTypes.GivenName,
            JwtClaimTypes.MiddleName,
            JwtClaimTypes.FamilyName,
            JwtClaimTypes.Role
        };

        addClaimActions.ForEach(claimAction => claimActions.Add(action: new UserClaimAction(claimAction)));
    }

    private static void RemoveUnwantedClaimActions(this ClaimActionCollection claimActions)
    {
        claimActions.DeleteClaim("idp");
        claimActions.DeleteClaim("nonce");
        claimActions.DeleteClaim("at_hash");
    }
}
