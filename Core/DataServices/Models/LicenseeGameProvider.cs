using System;

namespace TGS.Core.DataServices.Models
{
    public class LicenseeGameProvider
    {
        public Guid LicenseeId { get; set; }

        public Guid GameProviderId { get; set; }

        public Licensee Licensee { get; set; }

        public GameProvider GameProvider { get; set; }
    }
}