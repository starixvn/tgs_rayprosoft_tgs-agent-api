using System;

namespace TGS.Core.DataServices.Models
{
    public class LicenseeGame
    {
        public Guid GameId { get; set; }

        public virtual Game Game { get; set; }

        public Guid LicenseeId { get; set; }

        public virtual Licensee Licensee { get; set; }
    }
}