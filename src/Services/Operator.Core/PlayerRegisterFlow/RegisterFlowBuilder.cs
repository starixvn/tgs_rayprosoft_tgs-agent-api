using System;
using System.Collections.Immutable;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;

namespace TGS.Services.Operator.PlayerRegisterFlow;

internal class RegisterFlowBuilder
{
    private readonly IImmutableDictionary<string, Type> _registerFlowStepTypes;
    private readonly IServiceProvider _serviceProvider;

    public RegisterFlowBuilder(IServiceProvider serviceProvider, IImmutableDictionary<string, Type> registerFlowStepTypes)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _registerFlowStepTypes = registerFlowStepTypes ?? throw new ArgumentNullException(nameof(registerFlowStepTypes));
    }

    public RegisterFlowDefinition LoadSettings(JsonDocument settingsJson)
    {
        var definition = new RegisterFlowDefinition();

        definition.AddStep(_serviceProvider.GetRequiredService<CreatePlayerRegisterFlowStep>());

        foreach (var element in settingsJson.RootElement.EnumerateArray())
        {
            var type = element.GetProperty("type").GetString();

            if (string.IsNullOrWhiteSpace(type))
                throw new InvalidOperationException("RegisterFlowStep type is not specified");

            var step = (IRegisterFlowStep)_serviceProvider.GetRequiredService(_registerFlowStepTypes[type]);

            step.SetupStep(element);

            definition.AddStep(step);
        }

        return definition;
    }
}
