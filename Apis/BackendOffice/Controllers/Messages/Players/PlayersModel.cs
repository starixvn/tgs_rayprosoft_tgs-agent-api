using System;

namespace TGS.Apis.BackendOffice.Controllers.Messages.Players
{
    public class PlayerModel
    {
        public string Username { get; set; }
        public string Realname { get; set; }
        public string VipLevel { get; set; }
        public string ParentUsername { get; set; }
        public DateTimeOffset RegisteredOn { get; set; }
        public double Balance { get; set; }
        public string Status { get; set; }
        public bool HasAgent { get; set; }

        public string Phone { get; set; }
        public string Email { get; set; }
        public string QQ { get; set; }
        public string WeChat { get; set; }

        public BankModel[] Banks { get; set; }

        public class BankModel
        {
            public string Realname { get; set; }
            public string BankName { get; set; }
            public string BankAccount { get; set; }
        }
    }
}
