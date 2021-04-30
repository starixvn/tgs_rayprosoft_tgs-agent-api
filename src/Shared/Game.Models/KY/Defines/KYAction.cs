using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.KY.Defines
{
    /// <summary>
    /// KY Action
    /// </summary>
    public enum KYAction
    {
        /// <summary>
        /// The get game URL
        /// </summary>
        GetGameUrl = 0,
        /// <summary>
        /// The get balance
        /// </summary>
        GetBalance = 1,
        /// <summary>
        /// The fnud out(Tgs -> Gp)
        /// </summary>
        FnudOut = 2,
        /// <summary>
        /// The fnud in(GP -> Tgs)
        /// </summary>
        FnudIn = 3,
        /// <summary>
        /// The fund checking
        /// </summary>
        FundChecking = 4,
        /// <summary>
        /// The kick
        /// </summary>
        Kick = 8,
        /// <summary>
        /// The game record
        /// </summary>
        GameRecord = 6
    }
}
