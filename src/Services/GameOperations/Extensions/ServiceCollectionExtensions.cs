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
            services.AddScoped<IGameActionDataOperations, GameActionDataOperations>();
            services.AddScoped<IGameActionProcessor, GameActionProcessor>();

            services.AddScoped<IGamePlayActions, GamePlayActions>();
            services.AddScoped<IGameRoundsProcessor, GameRoundsProcessor>();

            services.AddScoped<IGameAccessor, GameAccessor>();

            services.AddSingleton<IActionWithinRoundValidator, ActionWithinRoundValidator>();
            services.AddSingleton<ITransactionSettingsGenerator, TransactionSettingsGenerator>();
            services.AddSingleton<IRoundStatusManager, RoundStatusManager>();

            services.AddSingleton<IBetValuesCalculator, DefaultBetValuesCalculator>();

            services
                .AddTransient<IRedisPlayerLoginRecordService>(sp => ActivatorUtilities.CreateInstance<RedisPlayerLoginRecordService>(sp, redisDatabaseResolver(sp)));

            services.AddScoped<ISingleWalletMessageSender, SingleWalletMessageSender>();

            return services;
        }
    }
}
