using System;
using MediatR;
using TGS.Core.Models.Messages;

namespace TGS.Services.BackOffice.Application.Models.PlayerBank
{
    public class UpdatePlayerBankRequest : PlayerBankRequest, IRequest<MessageResponse>
    {
        public Guid Id { get; set; }
    }
}