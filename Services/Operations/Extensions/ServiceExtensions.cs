using System;
using Microsoft.Extensions.DependencyInjection;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Services.Operations.CodeMappers;
using TGS.Services.Operations.Queries;
using TGS.Services.Operations.Validators;

namespace TGS.Services.Operations.Extensions
{
    public static class ServiceExtensions
    {
        /// <summary>
        /// Dependency Injection of Operations, need to add the following dependencies beforehead:
        /// 1. AddRedis() in Infrastructures.Redis        
        /// </summary>
        public static IServiceCollection AddOperations(this IServiceCollection services)
        {            
            #region Code Mappers

            services.AddSingleton<IGameProviderCurrencyCodeMapper, GameProviderCurrencyCodeMapper>();
            services.AddSingleton<IGameProviderLanguageCodeMapper, GameProviderLanguageCodeMapper>();

            #endregion

            #region Queries
                        
            services.AddFactory<ILicenseeQueries, LicenseeQueries>();
            services.AddFactory<IGameProviderQueries, GameProviderQueries>();
            services.AddFactory<IPlayerQueries, PlayerQueries>();
            services.AddFactory<IWalletQueries, WalletQueries>();

            #endregion

            #region Operations

            services.AddSingleton<ILicenseeOperations, LicenseeOperations>();
            services.AddSingleton<IGameProviderOperations, GameProviderOperations>();
            services.AddSingleton<IPlayerOperations, PlayerOperations>();
            services.AddSingleton<ILicenseeGameProviderOperations, LicenseeGameProviderOperations>();

            #endregion

            services.AddSingleton<IValidStatus<Status>, ValidStatus>();
            services.AddSingleton<IValidStatus<GameStatus>, ValidGameStatus>();
            services.AddSingleton<IStatusValidator, StatusValidator>();
            services.AddSingleton<IExpirationValidator, ExpirationValidator>();
            services.AddSingleton<IAmountValidator, AmountValidator>();

            services.AddScoped<ITransactionManager, TransactionManager>();
           
            return services;
        }

        private static void AddFactory<TInterface, TServices>(this IServiceCollection services)
            where TInterface : class
            where TServices : class, TInterface
        {
            services.AddScoped<TInterface, TServices>();
            services.AddTransient<Func<TInterface>>(c => () => c.GetService<TInterface>());
        }
    }
}