using System;
using MediatR;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;

namespace TGS.Services.Promotion
{
    public class UpdatePlayerRequest : IRequest<MessageResponse>
    {
        public string AgentUsername { get; init; }

        public string ConfirmedPassword { get; init; }

        public string EditorUsername { get; set; }

        public string Email { get; init; }

        public string Password { get; init; }

        public string Phone { get; init; }

        public string PlayerCode { get; set; }

        public string QQ { get; init; }

        public string RealName { get; init; }

        public PlayerStatus Status { get; init; }

        public string Type { get; set; }

        public Guid VipId { get; init; }

        public string Wechat { get; init; }
    }
}
