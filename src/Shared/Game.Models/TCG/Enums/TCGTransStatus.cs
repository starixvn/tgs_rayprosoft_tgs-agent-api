using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.TCG.Enums
{
    public enum TCGTransStatus
    {
        /// <summary>
        /// 未知
        /// </summary>
        UNKNOWN,
        /// <summary>
        /// 未找到
        /// </summary>
        NOT_FOUND,
        /// <summary>
        /// 失败
        /// </summary>
        FAILED,
        /// <summary>
        /// 成功
        /// </summary>
        SUCCESS,
        /// <summary>
        /// 待处理
        /// </summary>
        PENDING
    }
}
