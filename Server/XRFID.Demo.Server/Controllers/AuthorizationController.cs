using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace XRFID.Demo.Server.Controllers;

[ApiController]
public class AuthorizationController : ControllerBase
{
    private readonly IOpenIddictApplicationManager applicationManager;
    private readonly IOpenIddictScopeManager scopeManager;

    public AuthorizationController(IOpenIddictApplicationManager applicationManager, IOpenIddictScopeManager scopeManager)
    {
        this.applicationManager = applicationManager;
        this.scopeManager = scopeManager;
    }

    [HttpPost("~/connect/token")]
    [IgnoreAntiforgeryToken]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> Connect()
    {
        OpenIddictRequest? clientRequest = HttpContext.GetOpenIddictServerRequest();
        if (clientRequest is null || clientRequest.ClientId is null || !clientRequest.IsClientCredentialsGrantType())
        {
            return BadRequest();
        }

        var clientApplication = await applicationManager.FindByClientIdAsync(clientRequest.ClientId);
        if (clientApplication is null)
        {
            return NotFound();
        }

        ClaimsIdentity identity = new ClaimsIdentity(TokenValidationParameters.DefaultAuthenticationType, Claims.Name, Claims.Role);
        identity.SetClaim(Claims.Subject, await applicationManager.GetClientIdAsync(clientApplication));
        identity.SetClaim(Claims.Name, await applicationManager.GetDisplayNameAsync(clientApplication));
        identity.SetScopes(clientRequest.GetScopes());
        identity.SetResources(scopeManager.ListResourcesAsync(identity.GetScopes()).ToBlockingEnumerable().ToList());
        identity.SetDestinations(GetDestinations);

        return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    private static IEnumerable<string> GetDestinations(Claim claim)
    {
        return claim.Type switch
        {
            Claims.Name or
            Claims.Subject
                => new[] { Destinations.AccessToken, Destinations.IdentityToken },

            _ => new[] { Destinations.AccessToken },
        };
    }
}