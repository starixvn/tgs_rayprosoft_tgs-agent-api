using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.BigGaming.Enums
{
    public enum BGOrderStatus
    {
        /// <summary>
        /// 注单不存在 - 仅对免转模式有效
        /// </summary>
        [Description("注单不存在")]
        BET_SLIP_DOES_NOT_EXIST = 0,
        /// <summary>
        /// 未结算
        /// </summary>
        [Description("未结算")]
        RUNNING = 1,
        /// <summary>
        /// 结算赢 - 正常结算
        /// </summary>
        [Description("结算赢")]
        WIN = 2,
        /// <summary>
        /// 结算和 - 正常结算
        /// </summary>
        [Description("结算和")]
        TIE = 3,
        /// <summary>
        /// 结算输 - 正常结算
        /// </summary>
        [Description("结算输")]
        LOSE = 4,
        /// <summary>
        /// 取消 - 用户取消
        /// </summary>
        [Description("取消")]
        CANCEL = 5,
        /// <summary>
        /// 过期
        /// </summary>
        [Description("过期")]
        EXPIRED = 6,
        /// <summary>
        /// 系统取消
        /// </summary>
        [Description("系统取消")]
        SYSTEM_CANCEL = 7
    }
}
