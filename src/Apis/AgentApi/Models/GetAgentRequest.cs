using MediatR;

namespace TGS.Apis.AgentApi.Models
{
    public record GetAgentRequest : IRequest<GetAgentResponse>
    {
    }
}