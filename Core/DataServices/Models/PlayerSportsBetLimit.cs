using System;

namespace TGS.Core.DataServices.Models
{
    public class PlayerSportsBetLimit
    {
        public string PlayerCode { get;set;}

        public virtual Player Player { get; set; }

        public Guid SportMarketTypeId { get; set; }

        public virtual SportMarketType SportMarketType { get; set; }

        public int MinBet { get; set; }

        public int MaxBet { get; set; }

        public int MaxBetPerMatch { get; set; }
    }
}
