namespace Microservices.ReverseProxy.Auth;

public static class Auth0
{
    public static void AddAuth0(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication();
    }
        
    public static void AddAuth0(this WebApplication app)
    {
        app.UseAuthentication();
        app.MapGet("/auth", () => "Auth");
    }
}