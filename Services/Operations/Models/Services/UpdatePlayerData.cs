using System;

namespace TGS.Services.Operations.Models.Services
{
    public record UpdatePlayerData
    {
        public string Username { get; init; }
        public string PlayerCode { get; init; }
        public string RealName { get; init; }
        public string Phone { get; init; }
        public string Email { get; init; }
        public string QQ { get; init; }
        public string Wechat { get; init; }
        public DateTimeOffset UpdatedOn { get; init; }
    }
}