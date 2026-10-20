using CambioMoneda.Application.Interfaces;
using CambioMoneda.Application.Services;
using CambioMoneda.Infrastructure.Persistence;
using CambioMoneda.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CambioMoneda.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(o => o.UseSqlite(config.GetConnectionString("Default")));
        services.AddScoped<ITipoCambioRepository, TipoCambioRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ITipoCambioService, TipoCambioService>();
        services.AddScoped<IConversionService, ConversionService>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        return services;
    }
}