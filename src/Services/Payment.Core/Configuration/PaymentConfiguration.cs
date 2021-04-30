using Microsoft.Extensions.DependencyInjection;

namespace TGS.Services.Payment.Configuration
{
    public class PaymentConfiguration
    {
        public IServiceCollection Services { get; }

        public PaymentConfiguration(IServiceCollection services)
        {
            Services = services ?? throw new System.ArgumentNullException(nameof(services));
        }

        public PaymentConfiguration UsePaymentRepository<TPaymentRepository>() where TPaymentRepository : class, IPaymentRepository
        {
            Services
                .AddTransient<IPaymentRepository, TPaymentRepository>();

            return this;
        }
    }
}
