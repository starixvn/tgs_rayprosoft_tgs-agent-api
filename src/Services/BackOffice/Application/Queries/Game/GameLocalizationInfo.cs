using System.Collections.Generic;

namespace TGS.Services.BackOffice.Application.Queries.Game
{
    public class GameLocalizationInfo
    {
        public IEnumerable<string> Languages { get; set; }

        public IEnumerable<GameData> Games { get; set; }
    }
}