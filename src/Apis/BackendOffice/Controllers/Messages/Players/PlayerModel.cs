using System;
using TGS.Core.Models.Enums;

namespace TGS.Apis.BackendOffice.Controllers.Messages.Players
{
    public class PlayerModel
    {
        public string Username { get; set; }
        public string RealName { get; set; }
        public string VipLevel { get; set; }
        public string ParentUsername { get; set; }
        public DateTimeOffset RegisteredOn { get; set; }
        public decimal Balance { get; set; }
        public PlayerStatus Status { get; set; }
        public string StatusName { get; set; }
        public bool HasAgent { get; set; }

        public string Phone { get; set; }
        public string Email { get; set; }
        public string QQ { get; set; }
        public string WeChat { get; set; }

        public BankModel[] Banks { get; set; }

        public class BankModel
        {
            public Guid Id { get; set; }
            public string RealName { get; set; }
            public string BankName { get; set; }
            public string BankAccount { get; set; }
        }
    }
}