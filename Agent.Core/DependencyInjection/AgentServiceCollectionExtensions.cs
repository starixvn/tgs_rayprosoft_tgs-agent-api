using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using Microsoft.Extensions.Options;
using TGS.Core.Infrastructures.Mail;
using TGS.Services.Agent;
using TGS.Services.Agent.ApplicationServices.Player.PlayerRegistrationConditions;
using TGS.Services.Agent.Configuration;
using TGS.Services.Agent.ValidateCode;

namespace Microsoft.Extensions.DependencyInjection;

public static class AgentServiceCollectionExtensions
{
    private static readonly IImmutableDictionary<string, Type> _registrationTypes = new Dictionary<string, Type>
    {
        { "InitPhone", typeof(CheckPhoneCondition)},
        { "InitMail", typeof(CheckEmailCondition) },
        { "SetPhoneDontCareDuplicate", typeof(CheckPhoneDoNotCareDuplicateCondition) },
        { "SetEmailDontCareDuplicate", typeof(CheckEmailDoNotCareDuplicateCondition) }
    }.ToImmutableDictionary();

    public static AgentConfiguration AddAgentApiCore(this IServiceCollection services)
    {
        services.RegisterAllServiceUtilityTypes(
            Assembly.GetExecutingAssembly(),
            ServiceLifetime.Transient);

        services
            .AddOptions<AgentOptions>()
            .Services
            .AddValidateCodeClient((options, sp) =>
            {
                var agentOptions = sp.GetRequiredService<IOptions<AgentOptions>>().Value;

                options.ValidateCodeBaseUrl = agentOptions.ValidateCodeBaseUrl;
            })
            .AddTransient<IValidateCodeOperator, RestValidateCodeOperator>()
            .AddSingleton<IValidateCodeGenerator, RandomValidateCodeGenerator>()
            .AddMail()
            .AddSmsClient((sp, httpClient) =>
            {
                var options = sp.GetRequiredService<IOptions<AgentOptions>>().Value;

                httpClient.BaseAddress = options.SmsBaseUrl;
            });

        foreach (var type in _registrationTypes)
        {
            services.AddTransient(type.Value);
        }

        services.AddTransient<Func<string, IRegistrationCondition?>>(serviceProvider => key =>
        {
            _ = _registrationTypes.TryGetValue(key, out var distributionType);

            return distributionType == null
                ? null
                : (IRegistrationCondition?)serviceProvider.GetService(distributionType);
        });

        var agentConfiguration = new AgentConfiguration(services);

        return agentConfiguration;
    }
}
