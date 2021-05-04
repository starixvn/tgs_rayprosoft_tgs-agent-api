using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.Shared.Game.ApiClients.MultiWallet.Defines
{
    public enum GPAction
    {
        /// <summary>
        /// The get game URL
        /// </summary>
        GetGameUrl,
        /// <summary>
        /// The get balance
        /// </summary>
        GetBalance,
        /// <summary>
        /// The fnud out(Tgs -> Gp)
        /// </summary>
        FnudOut,
        /// <summary>
        /// The fnud in(GP -> Tgs)
        /// </summary>
        FnudIn,
        /// <summary>
        /// The fund checking
        /// </summary>
        FundChecking,
        /// <summary>
        /// The kick
        /// </summary>
        Kick,
        /// <summary>
        /// The game record
        /// </summary>
        GameRecord,
        /// <summary>
        /// The create player
        /// </summary>
        CreatePlayer,
    }
}
