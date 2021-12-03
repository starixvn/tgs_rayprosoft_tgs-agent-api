using System;
using TGS.Core.Models.Enums;

namespace TGS.Services.Operator.Services.Game
{
    public record GetBetConstructHtmlContentQuery(string PlayerCode, Guid BrandId, PlatformType PlatformType, string CultureName);
}
