using System;

namespace TGS.Core.DataServices.Models
{
    public class ResellerGameProvider
    {
        public Guid ResellerId { get; set; }

        public Guid GameProviderId { get; set; }

        public Reseller Reseller { get; set; }

        public GameProvider GameProvider { get; set; }
    }
}