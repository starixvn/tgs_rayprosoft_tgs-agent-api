using Tgs.Payment.Configuration;
using TGS.Payment.Repositories;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class PaymentConfigurationExtensions
    {
        public static PaymentConfiguration UseTgsPaymentRepository(this PaymentConfiguration paymentConfiguration)
            => paymentConfiguration.UsePaymentRepository<PaymentRepository>();
    }
}
