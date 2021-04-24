using System;
using MediatR;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;

namespace TGS.Services.BackOffice.Application.Models.Player
{
    public class UpdatePlayerRequest : BaseRequest, IRequest<MessageResponse>
    {
        public string PlayerCode { get; set; }
        public string Type { get; set; }
        public string RealName { get; init; }
        public Guid VipId { get; init; }
        public string AgentUsername { get; init; }
        public PlayerStatus Status { get; init; }
        public string Phone { get; init; }
        public string Email { get; init; }
        public string QQ { get; init; }
        public string Wechat { get; init; }
        public string Password { get; init; }
        public string ConfirmedPassword { get; init; }
    }
}