#:sdk Microsoft.NET.Sdk.Web

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();
app.MapGet("/", () => "Hola Mundo!");

app.Run();