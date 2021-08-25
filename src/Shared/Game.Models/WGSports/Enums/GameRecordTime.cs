using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.WGSports.Enums
{
    public enum GameRecordTime
    {
        /// <summary>
        /// 下注时间
        /// </summary>
        [Description("bet_date")]
        BetDate,
        /// <summary>
        /// 结算时间
        /// </summary>
        [Description("settle_date")]
        SettleDate
    }
}
