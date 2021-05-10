using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.Core.Models.Enums
{
    /// <summary>
    /// 資金來源
    /// </summary>
    public enum FundSource
    {
        /// <summary>
        /// 轉出
        /// Tgs -> GP
        /// </summary>
        FundOut = 1,
        /// <summary>
        /// 轉入
        /// GP -> Tgs 
        /// </summary>
        FundIn = 2,
    }
}
