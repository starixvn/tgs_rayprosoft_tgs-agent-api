using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.Core.Models.Enums
{
    public enum TransactionGameActionType
    {
        GameBet = 11000,

        GameSettleWin = 12100,
        GameSettleLose = 12200,
        GameSettleFree = 12300,
        GameSettleTie = 12400,
        GameWinJackpot = 12500,

        GameCancel = 13000,

        GameLock = 14100,
        GameUnlock = 14200,
    }
}
