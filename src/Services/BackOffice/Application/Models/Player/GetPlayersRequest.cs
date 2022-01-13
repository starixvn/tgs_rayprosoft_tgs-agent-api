using System;
using MediatR;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;

#nullable enable

namespace TGS.Services.BackOffice.Application.Models.Player
{
    public class GetPlayersRequest : PageableRequest, IRequest<PagedResponse<PlayersModel>>
    {
        public DateTimeOffset? BeginRegisteredOn { get; init; }

        public Guid? BrandId { get; init; }

        public DateTimeOffset? EndRegisteredOn { get; init; }

        public string? RegisterHost { get; init; }

        public PlayerStatus? Status { get; init; }

        public string? Username { get; init; }

        public Guid? VipId { get; init; }
    }
}
