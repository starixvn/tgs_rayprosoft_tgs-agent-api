using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.BigGaming.Enums
{
    public enum BigGamingModuleId
    {
        [Description("平台")]
        Lobby = 0,
        [Description("彩票模块")]
        Lottery = 1,
        [Description("视讯模块")]
        LiveDealer = 2,
        [Description("体育模块")]
        VirtualSports = 3,
    }
}
