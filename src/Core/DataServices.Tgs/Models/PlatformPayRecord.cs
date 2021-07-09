using System;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class PlatformPayRecord
    {
        public int Id { get; set; }
        public Guid PlatformCreditId { get; set; }
        public virtual PlatformCredit PlatformCredit { get; set; }
        public decimal PayAmount { get; set; }
        public decimal Balance { get; set; }
        public string CreatedByUsername { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
    }
}