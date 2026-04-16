using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using SteelWare.Presentation.Http.Controllers;

namespace SteelWare.Presentation.Http.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSteelWareHttpPresentation(this IServiceCollection services)
    {
        services.AddProblemDetails();
        services.AddOpenApi();
        services.AddControllers()
            .PartManager.ApplicationParts.Add(new AssemblyPart(typeof(SteelRollsController).Assembly));

        return services;
    }
}
