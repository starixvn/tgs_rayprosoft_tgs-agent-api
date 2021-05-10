using Microsoft.Extensions.DependencyInjection;

namespace TGS.Core.Infrastructures.Transactions
{
    public static class ServiceExtensions
    {
        /// <summary>
        ///     Dependency Injection of Transactions, need to add the following dependencies beforehand:
        ///     1. AddRepository of used database
        /// </summary>
        public static IServiceCollection AddTransactions(this IServiceCollection services)
        {
            services.AddTransient<ITransactionManager, TransactionManager>();
            services.AddTransient<SystemTransactionManager>();

            return services;
        }
    }
}