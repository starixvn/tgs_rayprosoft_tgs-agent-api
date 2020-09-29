using System;
using TGS.Core.Models.Enums;

namespace TGS.Services.Operations.Models.Services
{
    public record PlayerLogoutData
    {
        public string Username { get; init; }
        public string PlayerCode { get; init; }
        public PlatformType PlatformType { get; init; }
        public DateTimeOffset UpdatedOn { get; init; }
    }
}