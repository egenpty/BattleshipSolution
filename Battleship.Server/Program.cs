using Battleship.Server.Data;
using Battleship.Server.Models;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddSingleton<GameStateStore>();
builder.Services.AddScoped<GameService>();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Battleship API", Version = "v1" });
});

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

// Enable Swagger middleware
app.UseSwagger();

// Serve Swagger UI at root URL "/"
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Battleship API V1");
    c.RoutePrefix = string.Empty;  // Swagger UI served at "/"
});

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

var api = app.MapGroup("/api");

api.MapPost("/board", (GameService service) => service.CreateBoard());
api.MapPost("/board/{id}/ship", (Guid id, Ship ship, GameService service) =>
{
    return service.AddShip(id, ship) ? Results.Ok() : Results.BadRequest("Invalid ship placement.");
});

api.MapPost("/board/{id}/attack", (Guid id, Position pos, GameService service) =>
{
    var result = service.Attack(id, pos);
    return result is null ? Results.NotFound() : Results.Ok(result);
});

// Read port from environment or use 5000 as default
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
app.Urls.Add($"http://*:{port}");

app.Run();
