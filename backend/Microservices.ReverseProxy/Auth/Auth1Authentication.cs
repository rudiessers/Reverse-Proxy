namespace Microservices.ReverseProxy.Auth;

public static class Auth1Authentication
{
    public static void AddAuthentication(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication();
    }
        
    public static void AddAuthentication(this WebApplication app)
    {
        app.UseAuthentication();
        app.MapGet("/auth", () => "Auth");
    }
}