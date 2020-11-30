using System;

namespace TGS.Core.DataServices.Models
{
    public class ResellerGameProvider
    {
        public Guid ResellerId { get; set; }

        public Guid GameProviderId { get; set; }

        public virtual Reseller Reseller { get; set; }

        public virtual GameProvider GameProvider { get; set; }
    }
}