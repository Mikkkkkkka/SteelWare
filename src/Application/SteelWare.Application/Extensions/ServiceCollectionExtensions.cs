using Microsoft.Extensions.DependencyInjection;
using SteelWare.Application.Contracts;

namespace SteelWare.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSteelWareApplication(this IServiceCollection services)
    {
        services.AddScoped<ISteelRollPersistenceService, SteelRollPersistenceService>();
        services.AddScoped<IStorageStatisticsService, StorageStatisticsService>();

        return services;
    }
}