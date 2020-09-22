using System;
using TGS.Core.Models.Enums;

namespace TGS.Apis.OperatorApi.Controllers.Messages.Players
{
    public class UpdatePlayerRequest
    {
        public string RealName { get; set; }
        public Guid VipId { get; set; }
        public string ParentUsername { get; set; }
        public PlayerStatus Status { get; set; }
        public bool HasAgent { get; set; }


        public string Phone { get; set; }
        public string Email { get; set; }
        public string QQ { get; set; }
        public string Wechat { get; set; }


        public string Password { get; set; }
        public string ConfirmedPassword { get; set; }
    }
}
