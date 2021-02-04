using MediatR;

namespace TGS.Apis.OperatorApi.Models.Messages
{
    public record GetAnnouncementsRequest : IRequest<GetAnnouncementsResponse>
    {
    }
}
