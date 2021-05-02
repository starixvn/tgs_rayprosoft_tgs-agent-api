// unset

using System;
using System.Collections.Generic;
using TGS.Core.Models.Enums;
using JDBGameType = TGS.Shared.Game.Models.JDB.Enums.GameType;

namespace TGS.Apis.TgsApi.GameProviders.JDB.Services
{
    public class JDBGameTypeMapper
    {
        private static readonly Dictionary<GameType, Shared.Game.Models.JDB.Enums.GameType> _gameTypeMapper = new Dictionary<GameType, Shared.Game.Models.JDB.Enums.GameType>()
        {
            {GameType.Slot, JDBGameType.Slot},
            {GameType.Fishing, JDBGameType.Fish},
        };

        public JDBGameType Get(GameType gameType)
        {
            if(!_gameTypeMapper.TryGetValue(gameType, out var jdbGameType))
            {
                throw new ArgumentException($"JDB not support the game type:{gameType}");
            }

            return jdbGameType;
        }
    }
}