using TGS.Payment.Repositories;
using TGS.Services.Payment.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class PaymentConfigurationExtensions
    {
        public static PaymentConfiguration UseTgsPaymentRepository(this PaymentConfiguration paymentConfiguration)
            => paymentConfiguration.UsePaymentRepository<PaymentRepository>();
    }
}
