using System;
using MediatR;

namespace TGS.Services.BackOffice.Application.Models.GameProvider
{
    public class GetGameProviderBetLimitsRequest : IRequest<GetGameProviderBetLimitsResponse>
    {
        public GetGameProviderBetLimitsRequest(Guid gameProviderId)
        {
            GameProviderId = gameProviderId;
        }

        public Guid GameProviderId { get; set; }
    }
}