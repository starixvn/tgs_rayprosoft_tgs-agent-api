using System;
using MediatR;

namespace TGS.Services.BackOffice.Application.Models.GameProviderBetLimit
{
    public class GetGameProviderBetLimitsRequest : IRequest<GetGameProviderBetLimitsResponse>
    {
        public Guid GameProviderId { get; set; }
    }
}