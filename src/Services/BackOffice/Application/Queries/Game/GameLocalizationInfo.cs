using System.Collections.Generic;

namespace TGS.Services.BackOffice.Application.Queries.Game
{
    public class GameLocalizationInfo
    {
        public List<string> Languages { get; set; }

        public List<GameData> Games { get; set; }
    }
}