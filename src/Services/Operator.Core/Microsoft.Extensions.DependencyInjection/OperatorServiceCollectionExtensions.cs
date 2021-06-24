using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using TGS.Core.Infrastructures.Mail;
using TGS.Core.Infrastructures.WebApiServices;
using TGS.Payment.ManualDeposit;
using TGS.Services.Operator;
using TGS.Services.Operator.Configuration;
using TGS.Services.Operator.ValidateCode;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class OperatorServiceCollectionExtensions
    {
        public static OperatorConfiguration AddOperatorApiCore(this IServiceCollection services)
        {
            services.RegisterAllServiceUtilityTypes(
                typeof(OperatorServiceCollectionExtensions).Assembly,
                ServiceLifetime.Transient);

            services
                .AddOptions<OperatorOptions>()
                .Services
                .AddHttpClient<ValidateCodeServiceClient>()
                .ConfigureHttpClient((sp, httpClient) =>
                {
                    var options = sp.GetRequiredService<IOptions<OperatorOptions>>().Value;

                    httpClient.BaseAddress = options.ValidateCodeBaseUrl;
                })
                .Services
                .AddTransient<IGameProviderStatusValidator, GameProviderStatusValidator>()
                .AddTransient<IValidateCodeOperator, RestValidateCodeOperator>()
                .AddTransient<IValidateCodeServiceClient>(sp => sp.GetRequiredService<ValidateCodeServiceClient>())
                .AddTransient<IRequestAnalyzer, RequestAnalyzer>()
                .AddSingleton<IValidateCodeGenerator, RandomValidateCodeGenerator>()
                .AddMail()
                .AddSmsClient((sp, httpClient) =>
                {
                    var options = sp.GetRequiredService<IOptions<OperatorOptions>>().Value;

                    httpClient.BaseAddress = options.SmsBaseUrl;
                })
                .AddFileServiceClient((sp, httpClient) =>
                {
                    var options = sp.GetRequiredService<IOptions<OperatorOptions>>().Value;

                    httpClient.BaseAddress = options.FileServiceBaseUrl;
                })
                .AddPaymentClient()
                .AddSingleton<ManualDepositSerializer>()
                .AddNatsMessageQueue(config =>
                {
                    config.ConfigQueueOptions((options, sp) =>
                    {
                        var configuration = sp.GetRequiredService<IConfiguration>();

                        options.Url = configuration.GetValue<string>("NatsSetting:Urls");
                    });
                });

            var opConfiguration = new OperatorConfiguration(services);

            return opConfiguration;
        }
    }
}
