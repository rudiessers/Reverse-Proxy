using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using PFZW.Web.Services.Microservices.ReverseProxy.Jwt;


const string origins = "Localhost";

var builder = WebApplication.CreateBuilder(args);

////Jwt Raw Coding test. Om de jwt key te testen.
////builder.AddJwtAuthenticationTestConsumerAuth();

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


app.MapGet("/", () => message);

////Jwt Raw Coding test. Om de jwt key te testen.
////app.AddJwtAuthenticationTest();

app.MapReverseProxy();

app.UseCors(origins);

app.Run();