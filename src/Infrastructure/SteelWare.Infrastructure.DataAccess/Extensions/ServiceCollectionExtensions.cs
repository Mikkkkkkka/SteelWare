using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace SteelWare.Infrastructure.DataAccess.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddEFPostgresDatabase(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<SteelWareDbContext>(options => options.UseNpgsql(connectionString));
    }
}