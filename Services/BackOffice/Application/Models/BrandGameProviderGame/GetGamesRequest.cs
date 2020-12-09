using System;
using MediatR;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;

namespace TGS.Services.BackOffice.Application.Models.BrandGameProviderGame
{
    public class GetGamesRequest : PageableRequest, IRequest<PagedResponse<GameModel>>
    {
        public Guid? GameProviderId { get; set; }
        public GameType? GameType { get; set; }
        public string Keyword { get; set; }
    }
}