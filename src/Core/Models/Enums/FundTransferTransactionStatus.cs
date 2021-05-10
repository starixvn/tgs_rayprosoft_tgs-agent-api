using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.Core.Models.Enums
{
    public enum FundTransferTransactionStatus
    {
        /// <summary>
        /// The wait for response
        /// 建立訂單等待
        /// </summary>
        WaitForResponse = 1,
        /// <summary>
        /// The failure on game provider
        /// 遊戲供應失敗
        /// </summary>
        FailureOnGameProvider,
        /// <summary>
        /// The failure on system
        /// 主錢包失敗
        /// </summary>
        FailureOnSystem,
        /// <summary>
        /// The success
        /// 完成
        /// </summary>
        Success,
        /// <summary>
        /// The manuel fund in
        /// 手動補點回主錢包
        /// </summary>
        ManuelFundIn,
    }
}
