using System;

namespace TGS.Core.DataServices.Models
{
    public class BrandGame
    {
        public Guid GameId { get; set; }

        public Game Game { get; set; }

        public Guid BrandId { get; set; }

        public Brand Brand { get; set; }
    }
}