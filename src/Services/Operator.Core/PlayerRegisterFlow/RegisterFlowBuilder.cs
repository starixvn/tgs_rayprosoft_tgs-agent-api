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

        definition.AddStep(ActivatorUtilities.CreateInstance<CreatePlayerRegisterFlowStep>(_serviceProvider));

        foreach (var element in settingsJson.RootElement.EnumerateArray())
        {
            var type = element.GetProperty("type").GetString();

            if (string.IsNullOrWhiteSpace(type))
                throw new InvalidOperationException("RegisterFlowStep type is not specified");

            var step = (IRegisterFlowStep)ActivatorUtilities.CreateInstance(
                _serviceProvider,
                _registerFlowStepTypes[type]);

            definition.AddStep(step);
        }

        return definition;
    }
}
