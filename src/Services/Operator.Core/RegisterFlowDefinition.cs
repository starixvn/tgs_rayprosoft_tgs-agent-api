using System.Collections.Generic;

namespace TGS.Services.Operator;

internal class RegisterFlowDefinition
{
    private List<IRegisterFlowStep> _stepDefinitions = new();

    public void AddStep(IRegisterFlowStep step)
        => _stepDefinitions.Add(step);

    public RegisterFlow BuildRegisterFlow()
        => new RegisterFlow(_stepDefinitions);
}
