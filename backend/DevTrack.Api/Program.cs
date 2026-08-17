using System.Text;
using System.Text.Json.Serialization;
using DevTrack.Application;
using DevTrack.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DevTrack")
    ?? throw new InvalidOperationException("ConnectionStrings:DevTrack must be configured.");
var key = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Jwt:Key must be configured.");
if (Encoding.UTF8.GetByteCount(key) < 32)
    throw new InvalidOperationException("Jwt:Key must be at least 32 bytes.");
builder.Services.AddDbContext<DevTrackDbContext>(o => o.UseNpgsql(connectionString));
builder.Services.AddScoped<IAiLearningService, FallbackAiLearningService>();
builder.Services.AddControllers().AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer(); builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o => o.TokenValidationParameters = new() { ValidateIssuer = true, ValidateAudience = true, ValidateLifetime = true, ValidateIssuerSigningKey = true, ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "DevTrack.Api", ValidAudience = builder.Configuration["Jwt:Audience"] ?? "DevTrack.Web", IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), ClockSkew = TimeSpan.FromMinutes(1) });
builder.Services.AddAuthorization();
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins")
    .GetChildren()
    .Select(x => x.Value)
    .Where(x => !string.IsNullOrWhiteSpace(x))
    .Cast<string>()
    .ToList();
var legacyFrontendUrl = builder.Configuration["FrontendUrl"];
if (!string.IsNullOrWhiteSpace(legacyFrontendUrl) && !allowedOrigins.Contains(legacyFrontendUrl))
    allowedOrigins.Add(legacyFrontendUrl);
if (allowedOrigins.Count == 0 && builder.Environment.IsDevelopment())
    allowedOrigins.Add("http://localhost:5173");
if (allowedOrigins.Count == 0)
    throw new InvalidOperationException("Cors:AllowedOrigins or FrontendUrl must be configured.");
builder.Services.AddCors(options => options.AddPolicy("frontend", policy => policy
    .WithOrigins(allowedOrigins.ToArray())
    .WithHeaders("Content-Type", "Authorization")
    .WithMethods("GET", "POST", "PUT", "PATCH", "DELETE", "OPTIONS")));
builder.Services.AddProblemDetails();
var app = builder.Build();
app.UseExceptionHandler(); if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
app.UseCors("frontend"); app.UseAuthentication(); app.UseAuthorization(); app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "healthy" })); app.Run();
public partial class Program { }
