using System;
using MediatR;

namespace TGS.Services.BackOffice.Application.Models.BrandGameProviderScheduled
{
    public class GetDefaultTimesRequest : IRequest<GetDefaultTimesResponse>
    {
        public Guid BrandGameProviderId { get; set; }
    }
}