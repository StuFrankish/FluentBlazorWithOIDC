using System.IdentityModel.Tokens.Jwt;
using BlazorWebAppOidc;
using BlazorWebAppOidc.Components;
using Client.Extensions;
using IdentityModel;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

const string OIDC_SCHEME = OpenIdConnectDefaults.AuthenticationScheme;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(OIDC_SCHEME)
    .AddOpenIdConnect(OIDC_SCHEME, options =>
    {
        options.Events = new OpenIdConnectEvents
        {
            OnTokenValidated = async ctx =>
            {
                var accessToken = ctx.SecurityToken as JwtSecurityToken;
                if (accessToken != null)
                {
                    Console.WriteLine($"Access Token: {accessToken.RawData}");
                    foreach (var claim in accessToken.Claims)
                    {
                        Console.WriteLine($"Claim: {claim.Type} - {claim.Value}");
                    }
                }
                else
                {
                    Console.WriteLine("Access token is not available.");
                }
            }
        };

        options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

        options.Authority = "https://localhost:5001/";
        options.ClientId = "blazor.demo";
        options.ClientSecret = "secret";

        options.UsePkce = true;
        options.ResponseType = OpenIdConnectResponseType.Code;
        options.MapInboundClaims = false;
        options.SaveTokens = true;
        options.TokenValidationParameters.NameClaimType = JwtClaimTypes.Name;
        options.TokenValidationParameters.RoleClaimType = JwtClaimTypes.Role;

        options.GetClaimsFromUserInfoEndpoint = true;

        options.Scope.Clear();
        options.Scope.Add(OpenIdConnectScopes.OpenId);
        options.Scope.Add(OpenIdConnectScopes.Profile);

        options.ClaimActions.ApplyCustomClaimsActions();

    })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);

builder.Services.ConfigureCookieOidcRefresh(CookieAuthenticationDefaults.AuthenticationScheme, OIDC_SCHEME);

builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services
    .AddFluentUIComponents();

builder.Services.AddScoped<AuthenticationStateProvider, PersistingAuthenticationStateProvider>();

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(BlazorWebAppOidc.Client._Imports).Assembly);

app.MapGroup("/authentication").MapLoginAndLogout();

app.Run();
