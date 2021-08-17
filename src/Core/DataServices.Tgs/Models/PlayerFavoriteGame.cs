using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class PlayerFavoriteGame
    {
        public Guid BrandId { get; set; }

        public int GameId { get; set; }

        public GameType GameType { get; set; }

        public string PlayerCode { get; set; }

        public string SubGameProviderCode { get; set; }
    }
}
