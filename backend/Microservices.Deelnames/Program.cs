
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy();

var app = builder.Build();

var message = new
{
    msg = "Hello Deelnames!"
};

app.MapGet("/", () => message);


app.Run();
