using System;
using TGS.Core.Models.Enums;

namespace TGS.Services.BackOffice.Application.Models.PlayerOnline
{
    public class PlayerOnlineModel
    {
        public Guid BrandId { get; init; }
        public string PlayerCode { get; init; }
        public string Username { get; init; }
        public string RealName { get; init; }
        public PlayerStatus Status { get; init; }
        public Guid VipLevelId { get; init; }
        public int VipLevel { get; init; }
    }
}
