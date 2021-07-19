using System;
using System.Collections.Generic;
using TGS.Core.Models.Enums;

namespace TGS.Services.Operator
{
    public record GameProviderInfo(Guid Id, string Code, Status Status, bool IsTransferWallet,
        IEnumerable<GameTypeAndStatusInfo> Games, IEnumerable<string> Currencies);
}