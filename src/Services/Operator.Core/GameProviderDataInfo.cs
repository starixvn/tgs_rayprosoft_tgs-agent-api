using System;
using System.Collections.Generic;
using TGS.Core.Models.Enums;

namespace TGS.Services.Operator
{
    public record GameProviderDataInfo(Guid Id, string Code, Status Status, bool IsTransferWallet,
        IEnumerable<GameTypeAndStatusDataInfo> Games, IEnumerable<string> Currencies);
}