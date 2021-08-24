using TGS.BackOffice.Repositories;
using TGS.Services.BackOffice.Domain;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBackendOfficeRepositories(this IServiceCollection services)
            => services
                .AddTransient<ISettingRepository, SettingRepository>()
                .AddTransient<ISystemPaymentRepository, SystemPaymentRepository>()
                .AddTransient<IBackOfficeRepository, BackOfficeRepository>()
                .AddSingleton<ISystemClock, DefaultSystemClock>();
    }
}
