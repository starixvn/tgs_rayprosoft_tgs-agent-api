using TGS.BackOffice.Repositories;
using TGS.Services.BackOffice.Domain;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBackendOfficeRepositories(this IServiceCollection services)
        {
            services.AddTransient<IDepositRepository, DepositRepository>();
            services.AddTransient<ISettingRepository, SettingRepository>();
            services.AddTransient<ISystemPaymentRepository, SystemPaymentRepository>();

            return services;
        }
    }
}