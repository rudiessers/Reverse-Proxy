var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var message = new
{
    msg = "Hello Aanvragen!"
};

app.MapGet("/", () => message);


app.Run();