using MediatR;

namespace TGS.Apis.OperatorApi.Models.Messages
{
    public record LogoutRequest : IRequest<LogoutResponse>
    {
    }
}
