using System;

namespace TGS.Core.DataServices.Models
{
    public class LicenseeGame
    {
        public Guid GameId { get; set; }

        public Game Game { get; set; }

        public Guid LicenseeId { get; set; }

        public Licensee Licensee { get; set; }
    }
}