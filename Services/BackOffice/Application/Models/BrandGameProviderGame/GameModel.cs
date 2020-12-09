using TGS.Core.Models.Enums;

namespace TGS.Services.BackOffice.Application.Models.BrandGameProviderGame
{
    public class GameModel
    {
        public int DisplayOrder { get; set; }
        public int GameNumber { get; set; }
        public string GameName { get; set; }
        public GameType GameType { get; set; }
        public PlatformType Platform { get; set; }
        public GameTag GameTag { get; set; }
    }
}