using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace Microservices.ReverseProxy.Auth;

public static class Auth2Authorisation
{
    private const string AuthScheme = "cookie"; 
    private const string AuthScheme2 = "cookie2"; 
    
    public static void AddAuth2(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication(AuthScheme)
            .AddCookie(AuthScheme)
            .AddCookie(AuthScheme2);
    }
        
    public static void AddAuth2(this WebApplication app)
    {
        app.UseAuthentication();

        app.Use((ctx, next) =>
        {
            if (ctx.Request.Path.StartsWithSegments("/login"))
            {
                return next();
            }
            
            if (!ctx.User.Identities.Any(x => x.AuthenticationType == AuthScheme))
            {
                ctx.Response.StatusCode = 401;
                return Task.CompletedTask;
            }
            if (!ctx.User.HasClaim("passport_type", "eur"))
            {
                ctx.Response.StatusCode = 403;
                return Task.CompletedTask;
            }
            
            return next();
        });
        
        app.MapGet("/unsecure", (HttpContext ctx) =>
        {
            return ctx.User.FindFirst("usr")?.Value ?? "empty";
        });
        
        app.MapGet("/sweden", (HttpContext ctx) =>
        {
            // if (!ctx.User.Identities.Any(x => x.AuthenticationType == AuthScheme))
            // {
            //     ctx.Response.StatusCode = 401;
            //     return "";
            // }
            // if (!ctx.User.HasClaim("passport_type", "eur"))
            // {
            //     ctx.Response.StatusCode = 403;
            //     return "";
            // }

            return "allowed";
        });
        
        app.MapGet("/norway", (HttpContext ctx) =>
        {
            // if (!ctx.User.Identities.Any(x => x.AuthenticationType == AuthScheme))
            // {
            //     ctx.Response.StatusCode = 401;
            //     return "";
            // }
            // if (!ctx.User.HasClaim("passport_type", "eur"))
            // {
            //     ctx.Response.StatusCode = 403;
            //     return "";
            // }

            return "allowed";
        });
        
        // [AuthScheme(AuthScheme2)]
        // [AuthClaim("passport_type", "eur")]
        app.MapGet("/denmark", (HttpContext ctx) =>
        {
            // if (!ctx.User.Identities.Any(x => x.AuthenticationType == AuthScheme2))
            // {
            //     ctx.Response.StatusCode = 401;
            //     return "";
            // }
            // if (!ctx.User.HasClaim("passport_type", "eur"))
            // {
            //     ctx.Response.StatusCode = 403;
            //     return "";
            // }

            return "allowed";
        });
        
        app.MapGet("/login", async (HttpContext ctx) =>
        {
            var claims = new List<Claim> { new("usr", "anton")};
            claims.Add(new("passport_type", "eur") );
            var identity = new ClaimsIdentity(claims, AuthScheme);
            var user = new ClaimsPrincipal(identity);
            await ctx.SignInAsync(AuthScheme, user);
        });
    }
}