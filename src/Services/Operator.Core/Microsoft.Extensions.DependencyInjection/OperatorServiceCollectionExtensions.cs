using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using TGS.Core.Infrastructures.Mail;
using TGS.Core.Infrastructures.WebApiServices;
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
                .AddValidateCodeClient((options, sp) =>
                {
                    var operatorOptions = sp.GetRequiredService<IOptions<OperatorOptions>>().Value;

                    options.ValidateCodeBaseUrl = operatorOptions.ValidateCodeBaseUrl;
                })
                .AddTransient<IGameProviderStatusValidator, GameProviderStatusValidator>()
                .AddTransient<IValidateCodeOperator, RestValidateCodeOperator>()
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
                .AddPaymentClient();

            var opConfiguration = new OperatorConfiguration(services);

            return opConfiguration;
        }
    }
}