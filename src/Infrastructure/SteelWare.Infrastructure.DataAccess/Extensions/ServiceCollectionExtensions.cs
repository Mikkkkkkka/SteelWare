using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SteelWare.Application.Abstractions;
using SteelWare.Infrastructure.DataAccess.Repositories;

namespace SteelWare.Infrastructure.DataAccess.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSteelWareDataAccess(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("SteelWare")
                               ?? configuration["ConnectionStrings:DefaultConnection"]
                               ?? throw new InvalidOperationException(
                                   "Database connection string is missing. Configure ConnectionStrings:SteelWare.");

        return services.AddSteelWareDataAccess(connectionString);
    }

    public static IServiceCollection AddSteelWareDataAccess(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<SteelWareDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<ISteelRollRepository, SteelRollRepository>();

        return services;
    }
}