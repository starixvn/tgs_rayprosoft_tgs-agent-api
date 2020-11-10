using System;

namespace TGS.Core.DataServices.Models
{
    public class LicenseeGameProvider
    {
        public Guid LicenseeId { get; set; }

        public Guid GameProviderId { get; set; }

        public virtual Licensee Licensee { get; set; }

        public virtual GameProvider GameProvider { get; set; }
    }
}