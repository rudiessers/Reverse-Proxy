using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace Microservices.ReverseProxy.Auth;

public static class Auth2Authorisation
{
    private const string AuthScheme = "cookie"; 
    
    public static void AddAuth2(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication(AuthScheme)
            .AddCookie(AuthScheme);
    }
        
    public static void AddAuth2(this WebApplication app)
    {
        app.UseAuthentication();
        
        app.MapGet("/unsecure", (HttpContext ctx) =>
        {
            return ctx.User.FindFirst("usr").Value ?? "empty";
        });
        
        app.MapGet("/login", async (HttpContext ctx) =>
        {
            var claims = new List<Claim> { new("usr", "anton") };
            var identity = new ClaimsIdentity(claims, AuthScheme);
            var user = new ClaimsPrincipal(identity);
            await ctx.SignInAsync(AuthScheme, user);
        });
    }
}