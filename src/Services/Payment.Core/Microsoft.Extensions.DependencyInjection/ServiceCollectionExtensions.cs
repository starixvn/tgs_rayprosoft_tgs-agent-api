using TGS.Services.Payment.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static PaymentConfiguration AddPayment(this IServiceCollection services)
            => new PaymentConfiguration(services);
    }
}
