using TGS.Services.Payment;
using TGS.Services.Payment.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static PaymentConfiguration AddPayment(this IServiceCollection services)
        {
            services.AddSingleton<IWithdrawProcessorFactory, WithdrawProcessorFactory>();

            return new PaymentConfiguration(services);
        }
    }
}
