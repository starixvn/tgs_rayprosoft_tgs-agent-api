using MediatR;

namespace TGS.Apis.OperatorApi.Models.Messages
{
    public record CheckUsernameRequest : BaseUserRequest, IRequest<CheckUsernameResponse>
    {
    }
}