using System;
using System.Collections.Generic;
using TGS.Apis.TgsApi.GameProviders.Habanero;
using TGS.Apis.TgsApi.GameProviders.PragmaticPlay;
using TGS.Apis.TgsApi.GameProviders.Saba;
using TGS.Apis.TgsApi.GameProviders.SBOBET;
using TGS.Core.Models;

namespace TGS.Apis.TgsApi
{
    public partial class Program
    {
        // TODO: RedTiger is temporary used only since it has no implementation, would be removed or refactored in the future

        private static readonly IReadOnlyDictionary<string, Type> StartupMap = new Dictionary<string, Type>
        {
            {Consts.GameProvider.SBOBET, typeof(SBOBETStartup)},
            {Consts.GameProvider.Habanero, typeof(HabaneroStartup)},
            {Consts.GameProvider.PragmaticPlay, typeof(PragmaticPlayStartup)},
            {Consts.GameProvider.Saba, typeof(SabaStartup)}
            //{ Consts.GameProvider.RedTiger, typeof(RedTigerStartup) },
        };

        private static readonly IReadOnlyDictionary<string, string> ConfigFileMap = new Dictionary<string, string>
        {
            {Consts.GameProvider.SBOBET, "SBOBET/sbobet.json"},
            {Consts.GameProvider.Habanero, "Habanero/habanero.json"},
            {Consts.GameProvider.PragmaticPlay, "PragmaticPlay/pragmaticplay.json"},
            {Consts.GameProvider.Saba, "Saba/saba.json"}
            //{ Consts.GameProvider.RedTiger, "RedTiger/redtiger.json" }
        };
    }
}