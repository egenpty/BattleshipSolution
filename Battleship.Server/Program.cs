using Battleship.Server.Configuration;
using Battleship.Server.Data;
using Battleship.Server.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddSingleton<GameStateStore>();
builder.Services.AddScoped<GameService>();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Swagger + JWT support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Battleship API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter JWT with Bearer prefix",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// Bind JWT settings
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()
                  ?? throw new InvalidOperationException("JWT settings not configured");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Correct middleware order:
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Swagger middleware
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Battleship API V1");
    c.RoutePrefix = string.Empty;
});

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

// Token generation endpoint
app.MapPost("/login", (LoginRequest login) =>
{
    if (login.Username != "battle" || login.Password != "ready")
        return Results.Unauthorized();

    var claims = new[]
    {
        new Claim(ClaimTypes.Name, login.Username)
    };

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        issuer: jwtSettings.Issuer,
        audience: jwtSettings.Audience,
        claims: claims,
        expires: DateTime.UtcNow.AddMinutes(jwtSettings.ExpireMinutes),
        signingCredentials: creds
    );

    return Results.Ok(new
    {
        token = new JwtSecurityTokenHandler().WriteToken(token)
    });
})
.WithName("Login")
.WithTags("Auth");

// Protected API endpoints
var api = app.MapGroup("/api").RequireAuthorization();

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

// Read port from environment or default to 5000
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
app.Urls.Add($"http://*:{port}");

app.Run();
