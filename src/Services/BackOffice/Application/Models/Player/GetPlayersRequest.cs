using System;
using MediatR;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;

namespace TGS.Services.BackOffice.Application.Models.Player
{
    public class GetPlayersRequest : PageableRequest, IRequest<PagedResponse<PlayersModel>>
    {
        public Guid? BrandId { get; init; }
        public PlayerStatus? Status { get; init; }
        public Guid? VipId { get; init; }
        public string Username { get; init; }
    }
}