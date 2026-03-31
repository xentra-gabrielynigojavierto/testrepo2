var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello DevOps TEAM! test  31/03/2026");

app.Run();
