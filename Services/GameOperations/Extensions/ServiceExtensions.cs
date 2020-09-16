using Microsoft.Extensions.DependencyInjection;
using TGS.Services.GameOperations.Calculators;
using TGS.Services.GameOperations.Data;
using TGS.Services.GameOperations.Validators;

namespace TGS.Services.GameOperations.Extensions
{
    public static class ServiceExtensions
    {
        /// <summary>
        /// Dependency Injection of GameOperations, need to add dependencies beforehead:  
        /// 1. AddInfrastructures in Infrastructures project
        /// 2. AddTgsLog in Infrastructures.Log under in Infrastructures project
        /// 3. AddOperations in Operations project
        /// </summary>
        public static IServiceCollection AddGameOperations(this IServiceCollection services)
        {  
            services.AddScoped<IGameActionDataOperations, GameActionDataOperations>();
            services.AddScoped<IGamePlayActions, GamePlayActions>();

            services.AddSingleton<IActionWithinRoundValidator, ActionWithinRoundValidator>();
            services.AddSingleton<ITransactionSettingsGenerator, TransactionSettingsGenerator>();
            services.AddSingleton<IBetValuesCalculator, BetValuesCalculator>();
            services.AddSingleton<IRoundStatusManager, RoundStatusManager>();

            return services;
        }
    }
}