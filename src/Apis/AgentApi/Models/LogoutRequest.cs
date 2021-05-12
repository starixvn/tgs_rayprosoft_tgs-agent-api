using MediatR;

namespace TGS.Apis.AgentApi.Models
{
    public record LogoutRequest : IRequest<LogoutResponse>
    {
    }
}