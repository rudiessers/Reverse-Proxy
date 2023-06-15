using Microservices.ReverseProxy.Auth;

const string origins = "Localhost";

var builder = WebApplication.CreateBuilder(args);

// Auth
builder.AddAuth2();

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

// Auth
app.AddAuth2();

app.MapReverseProxy();

app.UseCors(origins);

app.Run();