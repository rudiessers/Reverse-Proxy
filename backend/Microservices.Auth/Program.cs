using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

var rsaKey = RSA.Create();
rsaKey.ImportRSAPrivateKey(File.ReadAllBytes("key"), out _);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication()
    .AddJwtBearer("jwt", o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateAudience = false,
            ValidateIssuer = false
        };
        o.Events = new JwtBearerEvents()
        {
            OnMessageReceived = (ctx) =>
            {
                if (ctx.Request.Query.ContainsKey("t"))
                {
                    ctx.Token = ctx.Request.Query["t"];
                }

                return Task.CompletedTask;
            }
        };
        o.Configuration = new OpenIdConnectConfiguration()
        {
            SigningKeys =
            {
                new RsaSecurityKey(rsaKey)
            }
        };

        o.MapInboundClaims = false;
    });

var app = builder.Build();

app.UseAuthentication();

app.MapGet("/", (HttpContext context) => context.User.FindFirst("sub")?.Value ?? "empty");
app.MapGet("/jwt", () =>
{
    var handler = new JsonWebTokenHandler();
    var key = new RsaSecurityKey(rsaKey);
    var token = handler.CreateToken(new SecurityTokenDescriptor()
    {
        Issuer = "http://localhost:5122",
        Subject = new ClaimsIdentity(new[]
        {
            new Claim("sub", Guid.NewGuid().ToString()),
            new Claim("name", "Anton")
        }),
        SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256)
    });

    return token;
});

app.MapGet("/jwk", () =>
{
    var publicKey = RSA.Create();
    publicKey.ImportRSAPublicKey(rsaKey.ExportRSAPublicKey(), out _);
    var key = new RsaSecurityKey(publicKey);
    return JsonWebKeyConverter.ConvertFromRSASecurityKey(key);
});

app.MapGet("/jwk-private", () =>
{
    var key = new RsaSecurityKey(rsaKey);
    return JsonWebKeyConverter.ConvertFromRSASecurityKey(key);
});

app.Run();