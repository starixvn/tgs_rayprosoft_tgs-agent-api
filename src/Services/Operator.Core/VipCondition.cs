#pragma warning disable CS8618 // 退出建構函式時，不可為 Null 的欄位必須包含非 Null 值。請考慮宣告為可為 Null。

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TGS.Services.Operator
{
    internal class VipCondition : IBrandPaymentCondition
    {
        public IEnumerable<Guid> AllowVips { get; init; }

        public ValueTask<bool> IsValidAsync(PlayerInfo player)
        {
            if (player.VipLevel == null)
                return ValueTask.FromResult(false);
            else
                return ValueTask.FromResult(AllowVips.Contains(player.VipLevel.Value));
        }
    }
}

#pragma warning restore CS8618 // 退出建構函式時，不可為 Null 的欄位必須包含非 Null 值。請考慮宣告為可為 Null。
