using System;
using System.Text.Json;
using TGS.Core.Models.Enums;

namespace TGS.Services.BackOffice.Application.Models.Player
{
    public class PlayerModel
    {
        public string AgentUsername { get; init; }

        public string AreaCode { get; init; }

        public decimal Balance { get; init; }

        public BankModel[] Banks { get; init; }

        public string Email { get; init; }

        public string LastLoginIp { get; init; }

        public DateTimeOffset? LastLoginOn { get; init; }

        public string Phone { get; init; }

        public string PlayerCode { get; init; }

        public string QQ { get; init; }

        public string RealName { get; init; }

        public DateTimeOffset? RegisteredOn { get; init; }

        public string RegisterIp { get; init; }

        public PlayerStatus Status { get; init; }

        public string StatusName { get; init; }

        public string Username { get; init; }

        public Guid? VipId { get; init; }

        public int? VipLevel { get; init; }

        public decimal WageringRequirement { get; set; }

        public string WeChat { get; init; }

        public class BankModel
        {
            public string BankType { get; init; }

            public JsonDocument Detail { get; init; }

            public Guid Id { get; init; }

            public Guid PaymentChannelId { get; init; }
        }
    }
}
