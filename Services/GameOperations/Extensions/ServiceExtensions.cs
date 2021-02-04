using Microsoft.Extensions.DependencyInjection;
using TGS.Services.GameOperations.Calculators;
using TGS.Services.GameOperations.Data;
using TGS.Services.GameOperations.Validators;

namespace TGS.Services.GameOperations.Extensions
{
    public static class ServiceExtensions
    {
        /// <summary>
        /// Dependency Injection of GameOperations, need to add dependencies beforehand:  
        /// 1. AddTimeZone() in Infrastructures.TimeZone
        /// 2. AddTgsLog() in Infrastructures.Log
        /// 3. AddOperations() in Operations project
        /// 4. AddTransactions() in Infrastructures.Transactions
        /// </summary>
        public static IServiceCollection AddGameOperations(this IServiceCollection services)
        {  
            services.AddScoped<IGameActionDataOperations, GameActionDataOperations>();
            services.AddScoped<IGameActionProcessor, GameActionProcessor>();
            services.AddScoped<IGamePlayActions, GamePlayActions>();

            services.AddSingleton<IActionWithinRoundValidator, ActionWithinRoundValidator>();
            services.AddSingleton<ITransactionSettingsGenerator, TransactionSettingsGenerator>();
            services.AddSingleton<IBetValuesCalculator, BetValuesCalculator>();
            services.AddSingleton<IRoundStatusManager, RoundStatusManager>();

            return services;
        }
    }
}