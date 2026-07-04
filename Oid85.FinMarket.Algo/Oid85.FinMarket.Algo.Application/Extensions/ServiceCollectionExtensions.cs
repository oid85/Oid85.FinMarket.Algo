using Microsoft.Extensions.DependencyInjection;
using Oid85.FinMarket.Algo.Application.Helpers;
using Oid85.FinMarket.Algo.Application.Interfaces.Services;
using Oid85.FinMarket.Algo.Application.Services;
using Oid85.FinMarket.Algo.Application.Strategies;
using Oid85.FinMarket.Algo.Core.Models;

namespace Oid85.FinMarket.Algo.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static void ConfigureApplicationServices(
        this IServiceCollection services)
    {
        services.AddScoped<IAlgoService, AlgoService>();
        services.AddScoped<IDataService, DataService>();

        services.AddScoped<AlgoHelper>();

        services.AddKeyedTransient<Strategy, UltimateSmootherInclinationLong>("UltimateSmootherInclinationLong");
    }
}