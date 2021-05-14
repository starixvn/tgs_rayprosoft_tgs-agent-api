using FluentValidation;
using TGS.Clients.Report.Client.Models.Agent;
using TGS.Core.Infrastructures.Extensions;

namespace TGS.Clients.Report.Client.Validations.Agent
{
    public class GetAgentContributionsRequestValidator : AbstractValidator<GetAgentContributionsRequest>
    {
        public GetAgentContributionsRequestValidator()
        {
            RuleFor(x => x.From)
                .LessThan(x => x.To)
                .WithMessage(Resources.Agent.ToDateGreaterThanFrom);

            RuleFor(p => p.Username)
                .Must((request, username) => request.AgentId is not null || username.IsNotNullOrEmpty())
                .WithMessage(Resources.Agent.AgentIdOrUsernameRequired);
        }
    }
}