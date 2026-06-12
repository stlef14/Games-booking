var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/", () => "API Scaffolding Active");

app.Run();

public partial class Program { }
