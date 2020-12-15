using System;

namespace TGS.Core.DataServices.Models
{
    public class BrandGroupGame
    {
        public Guid BrandGroupId { get; set; }
        public virtual BrandGroup BrandGroup { get; set; }
        public Guid BrandGameId { get; set; }
        public virtual BrandGame BrandGame { get; set; }
    }
}