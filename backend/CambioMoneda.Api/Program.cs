using System.Text;
using CambioMoneda.Api.Middleware;
using CambioMoneda.Infrastructure;
using CambioMoneda.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using CambioMoneda.Api.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(jwtKey) || jwtKey.Length < 32)
    throw new InvalidOperationException(
        "Falta Jwt:Key (mínimo 32 caracteres). Defínela como variable de entorno Jwt__Key.");
        
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(o => o.AddDocumentTransformer<BearerSecuritySchemeTransformer>());
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(o => o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        });

builder.Services.AddAuthorization();

builder.Services.AddCors(o => o.AddPolicy("Angular", p =>
    p.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
    scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreated();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    
}

app.UseMiddleware<ExceptionMiddleware>();

app.MapOpenApi();
app.UseSwaggerUI(o =>
{
    o.SwaggerEndpoint("/openapi/v1.json", "CambioMoneda API v1");
    o.RoutePrefix = "swagger";
});

app.UseCors("Angular");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
