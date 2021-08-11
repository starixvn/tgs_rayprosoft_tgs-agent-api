using System;
using MediatR;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;

namespace TGS.Services.Report.Application.Models.Round
{
    public class GetRoundsRequest : PageableRequest, IRequest<PagedResponse<RoundModel>>
    {
        public Guid BrandId { get; set; }
        public string Username { get; set; }
        public string PlayerCode { get; set; }
        public Guid? RoundId { get; set; }
        public string ExternalRoundId { get; set; }
        public Guid? GameProviderId { get; set; }
        public ClientRoundStatus? Status { get; set; }
        public GameType? GameType { get; set; }
        public DateTimeOffset From { get; set; }
        public DateTimeOffset To { get; set; }
    }
}