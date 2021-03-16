using System;
using MediatR;
using TGS.Core.Models.Messages;

namespace TGS.Services.BackOffice.Application.Models.PlayerBank
{
    public class DeletePlayerBankRequest : BaseRequest, IRequest<MessageResponse>
    {
        public DeletePlayerBankRequest(string playerCode, Guid id)
        {
            PlayerCode = playerCode;
            Id = id;
        }

        public Guid Id { get; set; }
        public string PlayerCode { get; set; }
    }
}