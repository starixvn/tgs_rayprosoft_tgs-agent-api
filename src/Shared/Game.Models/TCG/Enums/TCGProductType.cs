using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.TCG.Enums
{
    public enum TCGProductType
    {
        /// <summary>
        /// TCG LOTTO
        /// </summary>
        [Description("TCG LOTTO")]
        TCGLOTTO = 2,
        /// <summary>
        /// TCG_SEA
        /// </summary>
        [Description("TCG_SEA")]
        TCG_SEA = 384,
        /// <summary>
        /// TCG_LOTTO_VN
        /// </summary>
        [Description("TCG_LOTTO_VN")]
        TCG_LOTTO_VN = 420
    }
}
