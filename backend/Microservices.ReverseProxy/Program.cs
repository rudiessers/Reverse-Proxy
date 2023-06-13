using Microservices.ReverseProxy.Auth;

const string origins = "Localhost";

var builder = WebApplication.CreateBuilder(args);

//// Auth.12 - Raw Coding
builder.AddJwtAuthenticationTestConsumerAuth();

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
app.AddJwtAuthenticationTest();

app.MapReverseProxy();

app.UseCors(origins);

app.Run();