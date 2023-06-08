using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

var jwkString =
     "{\"additionalData\":{},\"alg\":null,\"crv\":null,\"d\":null,\"dp\":null,\"dq\":null,\"e\":\"AQAB\",\"k\":null,\"keyId\":null,\"keyOps\":[],\"kid\":null,\"kty\":\"RSA\",\"n\":\"p7mRzIjuwbXdLeMdJXH2Br1H6W3VYHx3AwTCDwQuEQSXxRetPfULY9_v965HudLwvRBVNXujGwRIjGygFrb7nEVcQMjO17f9rs2DFO1yWDlQllTIfkZvHRB83qPA-LmPXthh2TLd1omD5OJFU9w4KMEPPT0wGQw_IG2l-3Efx0etOHSWDwn3kp0Rhupj6DHOWWfTuz8TZm-L6EvNAQ_oxXGqnMtRMhsQjz-hVa_eIohNTfFdK68CEcBTLOlFc5sudgg89s6CY_ftdf8enZV2lbuJq67D4ifT026CkXs_GqSRdZqtHrW135pCD28lRmepoH80uS4bfS1ncn4BC5rCUQ\",\"oth\":null,\"p\":null,\"q\":null,\"qi\":null,\"use\":null,\"x\":null,\"x5c\":[],\"x5t\":null,\"x5tS256\":null,\"x5u\":null,\"y\":null,\"keySize\":2048,\"hasPrivateKey\":false,\"cryptoProviderFactory\":{\"cryptoProviderCache\":{},\"customCryptoProvider\":null,\"cacheSignatureProviders\":true,\"signatureProviderObjectPoolCacheSize\":32}}";
// var jwkString =
//     "{\"additionalData\":{},\"alg\":null,\"crv\":null,\"d\":null,\"dp\":null,\"dq\":null,\"e\":\"AQAB\",\"k\":null,\"keyId\":null,\"keyOps\":[],\"kid\":null,\"kty\":\"RSA\",\"n\":\"wbDiLbH2XwgFZ-QnTArbpmStCbllM9VxEy0r9OjdGf_qy81wibEJYqYi66qe9VYHUJGCrPlo5A7CkI5uQbKOHpJVRj6t3HLr9782MbyacNcTORKSvaNC9lypd8Bu1VntvT_eZGpU_R-zBZrpQqgQY2HrKSEEWRdtgOStkF_nQBeQcthTK7PA3YMCBMNjgK467KWtiJZfmjz7ttOXdLOkMwsJopSgC3-CVFmqexX6GkeU1FHwsfzM7CxPkD21QjjEOrd57KOi3izTcEKWf7imzY1pDBdwoBSru9BeuK7JsnH5yRPPWoFipH02j6-dlEOndmQaiS2tL_fX3mq-7cz9jQ\",\"oth\":null,\"p\":null,\"q\":null,\"qi\":null,\"use\":null,\"x\":null,\"x5c\":[],\"x5t\":null,\"x5tS256\":null,\"x5u\":null,\"y\":null,\"keySize\":2048,\"hasPrivateKey\":false,\"cryptoProviderFactory\":{\"cryptoProviderCache\":{},\"customCryptoProvider\":null,\"cacheSignatureProviders\":true,\"signatureProviderObjectPoolCacheSize\":32}}";

const string origins = "Localhost";

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
                JsonWebKey.Create(jwkString)
            }
        };

        o.MapInboundClaims = false;
    });

builder.Services.AddCors(options => 
    options.AddPolicy(name: origins,
        policy =>
        {
            policy
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        }));

// Initialize the reverse proxy from the "ReverseProxy" section of configuration
var proxyBuilder = builder.Services.AddReverseProxy();
proxyBuilder.LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

var message = new
{
    msg = "Hello Reverse Proxy!"
};

app.UseAuthentication();

app.MapGet("/", () => message);

app.MapGet("/token", (HttpContext context) => context.User.FindFirst("sub")?.Value ?? "empty");

app.MapGet("/jwt", () =>
{
    var handler = new JsonWebTokenHandler();
    var token = handler.CreateToken(new SecurityTokenDescriptor()
    {
        Issuer = "http://localhost:5122",
        Subject = new ClaimsIdentity(new[]
        {
            new Claim("sub", Guid.NewGuid().ToString()),
            new Claim("name", "Anton")
        }),
        SigningCredentials = new SigningCredentials(JsonWebKey.Create(jwkString), SecurityAlgorithms.RsaSha256)
    });

    return token;
});

// app.MapGet("/login", () => Results.SignIn(
//         new ClaimsPrincipal(
//             new ClaimsIdentity(
//                 new [] { new Claim("user_id", Guid.NewGuid().ToString())},
//                 "cookie"
//             )
//         ),
//         authenticationScheme: "cookie"
//     )
// );

app.MapReverseProxy();

app.UseCors(origins);

app.Run();
