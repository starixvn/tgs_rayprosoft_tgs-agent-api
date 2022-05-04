// ReSharper disable CheckNamespace

using System;
using StackExchange.Redis;
using TGS.Services.GameOperations;
using TGS.Services.GameOperations.Calculators;
using TGS.Services.GameOperations.Data;
using TGS.Services.GameOperations.Validators;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        ///     Dependency Injection of GameOperations, need to add dependencies beforehand:
        ///     1. AddTimeZone() in Infrastructures.TimeZone
        ///     2. AddTgsLog() in Infrastructures.Log
        ///     3. AddOperations() in Operations project
        ///     4. AddTransactions() in Infrastructures.Transactions
        /// </summary>
        public static IServiceCollection AddGameOperations(this IServiceCollection services,
            Func<IServiceProvider, IDatabase> redisDatabaseResolver)
        {
            _ = services.AddScoped<IGameActionDataOperations, GameActionDataOperations>();
            _ = services.AddScoped<IGameActionProcessor, GameActionProcessor>();

            _ = services.AddScoped<IGamePlayActions, GamePlayActions>();
            _ = services.AddScoped<IGameRoundsProcessor, GameRoundsProcessor>();

            _ = services.AddScoped<IGameAccessor, GameAccessor>();

            _ = services.AddSingleton<IActionWithinRoundValidator, ActionWithinRoundValidator>();

            _ = services.AddSingleton<ITransactionSettingsGenerator, TransactionSettingsGenerator>();
            _ = services.AddSingleton<IRoundStatusManager, RoundStatusManager>();

            _ = services.AddSingleton<IBetValuesCalculator, DefaultBetValuesCalculator>();

            _ = services
                .AddTransient<IRedisPlayerLoginRecordService>(sp => ActivatorUtilities.CreateInstance<RedisPlayerLoginRecordService>(sp, redisDatabaseResolver(sp)));

            _ = services.AddTransient<IExpirationValidator, ExpirationValidator>();

            return services;
        }
    }
}
