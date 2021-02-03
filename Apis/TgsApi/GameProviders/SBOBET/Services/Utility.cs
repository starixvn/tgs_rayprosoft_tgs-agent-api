using System;
using System.Collections.Generic;
using TGS.Core.Models.Enums;

namespace TGS.Apis.TgsApi.GameProviders.SBOBET.Services
{
    public static class Utility
    {
        private static readonly IReadOnlyDictionary<GameType, string> Portfolios = new Dictionary<GameType, string>
        {
            {GameType.Sportsbook, "SportsBook"},
            {GameType.VirtualSports, "VirtualSports"},
            {GameType.VirtualSportsLobby, "VirtualSports"}
        };

        public static string GetPortfolio(GameType gameType) =>
            !Portfolios.TryGetValue(gameType, out var portfolio)
                ? throw new InvalidOperationException($"No portfolio mapping is found: {gameType}")
                : portfolio;
    }
}
