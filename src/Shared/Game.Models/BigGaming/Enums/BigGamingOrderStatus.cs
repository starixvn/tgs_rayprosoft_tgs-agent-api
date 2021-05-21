using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.BigGaming.Enums
{
    public enum BigGamingOrderStatus
    {
        /// <summary>
        /// 注单不存在 - 仅对免转模式有效
        /// </summary>
        [Description("注单不存在")]
        BetSlipDoesNotExist = 0,
        /// <summary>
        /// 未结算
        /// </summary>
        [Description("未结算")]
        Running = 1,
        /// <summary>
        /// 结算赢 - 正常结算
        /// </summary>
        [Description("结算赢")]
        Win = 2,
        /// <summary>
        /// 结算和 - 正常结算
        /// </summary>
        [Description("结算和")]
        Tie = 3,
        /// <summary>
        /// 结算输 - 正常结算
        /// </summary>
        [Description("结算输")]
        Lose = 4,
        /// <summary>
        /// 取消 - 用户取消
        /// </summary>
        [Description("取消")]
        Cancel = 5,
        /// <summary>
        /// 过期
        /// </summary>
        [Description("过期")]
        Expired = 6,
        /// <summary>
        /// 系统取消
        /// </summary>
        [Description("系统取消")]
        SystemCancel = 7
    }
}
