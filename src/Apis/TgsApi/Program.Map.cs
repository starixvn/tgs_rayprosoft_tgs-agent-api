using System;
using System.Collections.Generic;
using TGS.Apis.TgsApi.GameProviders.AsiaGaming;
using TGS.Apis.TgsApi.GameProviders.BigGaming;
using TGS.Apis.TgsApi.GameProviders.Habanero;
using TGS.Apis.TgsApi.GameProviders.IMESports;
using TGS.Apis.TgsApi.GameProviders.KY;
using TGS.Apis.TgsApi.GameProviders.JDB;
using TGS.Apis.TgsApi.GameProviders.PragmaticPlay;
using TGS.Apis.TgsApi.GameProviders.Saba;
using TGS.Apis.TgsApi.GameProviders.SBOBET;
using TGS.Apis.TgsApi.GameProviders.VR;
using TGS.Core.Models;
using TGS.Apis.TgsApi.GameProviders.SexyGaming;
using TGS.Apis.TgsApi.GameProviders.Skywind;
using TGS.Apis.TgsApi.GameProviders.Evolution;

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
            {Consts.GameProvider.AsiaGaming, typeof(AsiaGamingStartup)},
            {Consts.GameProvider.JDB, typeof(JDBStartup)},
            {Consts.GameProvider.Saba, typeof(SabaStartup)},
            {Consts.GameProvider.KY, typeof(KYStartup)},
            {Consts.GameProvider.VR, typeof(VRStartup)},
            {Consts.GameProvider.IMESports, typeof(IMESportsStartup)},
            {Consts.GameProvider.BigGaming, typeof(BigGamingStartup)},
            {Consts.GameProvider.SexyGaming, typeof(SexyGamingStartup)},
            {Consts.GameProvider.Skywind, typeof(SkywindStartup)},
            {Consts.GameProvider.Evolution, typeof(EvolutionStartup)}
        };

        private static readonly IReadOnlyDictionary<string, string> ConfigFileMap = new Dictionary<string, string>
        {
            {Consts.GameProvider.SBOBET, "SBOBET/sbobet.json"},
            {Consts.GameProvider.Habanero, "Habanero/habanero.json"},
            {Consts.GameProvider.PragmaticPlay, "PragmaticPlay/pragmaticplay.json"},
            {Consts.GameProvider.AsiaGaming, "AsiaGaming/asiagaming.json"},
            {Consts.GameProvider.JDB, "JDB/jdb.json"},
            {Consts.GameProvider.Saba, "Saba/saba.json"},
            {Consts.GameProvider.KY, "KY/ky.json"},
            {Consts.GameProvider.VR, "VR/vr.json"},
            {Consts.GameProvider.BigGaming, "BigGaming/biggaming.json"},
            {Consts.GameProvider.IMESports, "IMESports/imesports.json"},
            {Consts.GameProvider.SexyGaming, "SexyGaming/sexygaming.json"},
            {Consts.GameProvider.Skywind, "Skywind/skywind.json"},
            {Consts.GameProvider.Evolution, "Evolution/evolution.json"}
        };
    }
}