using System;
using MediatR;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;

namespace TGS.Services.BackOffice.Application.Models.BrandAllAgent
{
    public record UpdateBrandAllAgentGameTypeRequest(Guid BrandId, bool Switch, bool ValidBet, bool WinLoss,
        AllAgentGameType GameType) : IRequest<MessageResponse>;
}