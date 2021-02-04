using MediatR;

namespace TGS.Apis.OperatorApi.Models.Messages
{
    public record GetPlayerRequest : IRequest<GetPlayerResponse>
    {
    }
}
