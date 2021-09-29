using System;
using System.Collections.Generic;
using TGS.Apis.TgsApi.GameProviders.AllBet;
using TGS.Apis.TgsApi.GameProviders.AsiaGaming;
using TGS.Apis.TgsApi.GameProviders.BigGaming;
using TGS.Apis.TgsApi.GameProviders.Card365;
using TGS.Apis.TgsApi.GameProviders.Evolution;
using TGS.Apis.TgsApi.GameProviders.Habanero;
using TGS.Apis.TgsApi.GameProviders.IMESports;
using TGS.Apis.TgsApi.GameProviders.JDB;
using TGS.Apis.TgsApi.GameProviders.KY;
using TGS.Apis.TgsApi.GameProviders.PragmaticPlay;
using TGS.Apis.TgsApi.GameProviders.Saba;
using TGS.Apis.TgsApi.GameProviders.SBOBET;
using TGS.Apis.TgsApi.GameProviders.SexyGaming;
using TGS.Apis.TgsApi.GameProviders.Skywind;
using TGS.Apis.TgsApi.GameProviders.TCG;
using TGS.Apis.TgsApi.GameProviders.VR;
using TGS.Apis.TgsApi.GameProviders.WGHorse;
using TGS.Apis.TgsApi.GameProviders.WGSports;
using TGS.Apis.TgsApi.GameProviders.WMCasino;
using static TGS.Core.Models.Consts.GameProvider;

namespace TGS.Apis.TgsApi
{
    public partial class Program
    {
        private static readonly IReadOnlyDictionary<string, Type> _StartupMap = new Dictionary<string, Type>
        {
            { SBOBET, typeof(SBOBETStartup) },
            { Habanero, typeof(HabaneroStartup) },
            { PragmaticPlay, typeof(PragmaticPlayStartup) },
            { AsiaGaming, typeof(AsiaGamingStartup) },
            { JDB, typeof(JDBStartup) },
            { Saba, typeof(SabaStartup) },
            { KY, typeof(KYStartup) },
            { VR, typeof(VRStartup) },
            { IMESports, typeof(IMESportsStartup) },
            { BigGaming, typeof(BigGamingStartup) },
            { SexyGaming, typeof(SexyGamingStartup) },
            { Skywind, typeof(SkywindStartup) },
            { AllBet, typeof(AllBetStartup) },
            { Evolution, typeof(EvolutionStartup) },
            { WGSports, typeof(WGSportsStartup) },
            { WGHorse, typeof(WGHorseStartup) },
            { WMCasino, typeof(WMCasinoStartup) },
            { TCG, typeof(TCGStartup) },
            { Card365, typeof(Card365Startup) }
        };

        private static readonly IReadOnlyDictionary<string, string> _ConfigFileMap = new Dictionary<string, string>
        {
            { SBOBET, "SBOBET/sbobet.json" },
            { Habanero, "Habanero/habanero.json" },
            { PragmaticPlay, "PragmaticPlay/pragmaticplay.json" },
            { AsiaGaming, "AsiaGaming/asiagaming.json" },
            { JDB, "JDB/jdb.json" },
            { Saba, "Saba/saba.json" },
            { KY, "KY/ky.json" },
            { VR, "VR/vr.json" },
            { BigGaming, "BigGaming/biggaming.json" },
            { IMESports, "IMESports/imesports.json" },
            { SexyGaming, "SexyGaming/sexygaming.json" },
            { Skywind, "Skywind/skywind.json" },
            { AllBet, "AllBet/allbet.json" },
            { Evolution, "Evolution/evolution.json" },
            { WGSports, "WGSports/wgsports.json" },
            { WGHorse, "WGHorse/wghorse.json" },
            { WMCasino, "WMCasino/wmcasino.json" },
            { TCG, "TCG/tcg.json" },
            { Card365, "Card365/card365.json" }
        };
    }
}