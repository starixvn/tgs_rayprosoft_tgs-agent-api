using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.Models
{
    public class BrandGame
    {
        public Guid GameId { get; set; }

        public virtual Game Game { get; set; }

        public Guid BrandId { get; set; }

        public virtual Brand Brand { get; set; }

        public GameStatus Status { get; set; }
    }
}