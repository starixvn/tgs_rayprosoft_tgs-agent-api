using System;
using MediatR;
using TGS.Core.Models.Messages;

namespace TGS.Services.BackOffice.Application.Models.Aggregate
{
    public record GenerateBrandGameProviderAggregateRequest
        (DateTimeOffset StartDate, Guid? BrandId, Guid UserId) : IRequest<MessageResponse>;
}