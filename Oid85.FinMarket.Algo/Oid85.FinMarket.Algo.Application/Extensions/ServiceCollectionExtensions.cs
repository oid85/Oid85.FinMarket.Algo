using Microsoft.Extensions.DependencyInjection;
using Oid85.FinMarket.Algo.Application.Interfaces.Services;
using Oid85.FinMarket.Algo.Application.Services;

namespace Oid85.FinMarket.Algo.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static void ConfigureApplicationServices(
        this IServiceCollection services)
    {
        services.AddTransient<IAlgoService, AlgoService>();
    }
}