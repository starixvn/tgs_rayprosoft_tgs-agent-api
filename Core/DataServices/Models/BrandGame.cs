using System;
using System.Collections.Generic;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.Models
{
    public class BrandGame
    {
        public Guid Id { get; set; }

        public int GameId { get; set; }

        public virtual Game Game { get; set; }

        public Guid BrandId { get; set; }

        public virtual Brand Brand { get; set; }

        public GameStatus Status { get; set; }

        public GameTag Tag { get; set; }

        public virtual ICollection<BrandGroupGame> BrandGroups {get;set;}
    }
}