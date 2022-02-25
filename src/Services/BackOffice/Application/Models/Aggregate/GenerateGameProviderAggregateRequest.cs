using System;
using MediatR;
using TGS.Core.Models.Messages;

namespace TGS.Services.BackOffice.Application.Models.Aggregate
{
    public record GenerateGameProviderAggregateRequest
        (DateTimeOffset StartDate, Guid? GameProviderId) : IRequest<MessageResponse>;
}