using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.Models
{
    public class Whitelist
    {
        public Guid Id { get; set; }
        public Guid? LicenseeId { get; set; }
        public Guid? ResellerId { get; set; }
        public virtual Licensee Licensee { get; set; }
        public virtual Reseller Reseller { get; set; }
        public string IpAddress { get; set; }
        public string Description { get; set; }
        public string InternalNote { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public string CreatedByUsername { get; set; }
        public string UpdatedByUsername { get; set; }
        public WhitelistType Type { get; set; }
    }
}