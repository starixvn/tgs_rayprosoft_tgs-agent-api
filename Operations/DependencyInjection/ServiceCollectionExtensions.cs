using System;
using Microsoft.Extensions.DependencyInjection;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Services.Operations.Commands;
using TGS.Services.Operations.Queries;
using TGS.Services.Operations.Validators;

namespace TGS.Services.Operations.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Dependency Injection of Operations, need to add the following dependencies beforehand:
    ///     1. AddRepository of Tgs database
    ///     2. AddRedis() in Infrastructures.Redis
    /// </summary>
    public static IServiceCollection AddOperations(this IServiceCollection services)
    {
        #region Queries

        services.AddFactory<IBrandQueries, BrandQueries>();
        services.AddFactory<IAgentQueries, AgentQueries>();
        services.AddFactory<IWalletQueries, WalletQueries>();

        #endregion Queries

        #region Commands

        services.AddFactory<IAgentCommands, AgentCommands>();

        #endregion Commands

        #region Operations

        services.AddScoped<IBrandOperations, BrandOperations>();
        services.AddScoped<IAgentOperations, AgentOperations>();

        #endregion Operations

        services.AddSingleton<IValidStatus<Status>, ValidStatus>();
        services.AddSingleton<IValidStatus<GameStatus>, ValidGameStatus>();

        services.AddScoped<IStatusValidator, StatusValidator>();

        return services;
    }

    private static void AddFactory<TInterface, TServices>(this IServiceCollection services)
        where TInterface : class
        where TServices : class, TInterface
    {
        services.AddScoped<TInterface, TServices>();
        services.AddTransient<Func<TInterface>>(c => c.GetRequiredService<TInterface>);
    }
}
