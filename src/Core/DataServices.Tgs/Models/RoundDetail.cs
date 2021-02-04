using System;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class RoundDetail
    {
        public Guid RoundId { get; set; }
        public virtual Round Round { get; set; }
        public string AdditionalData { get; set; }
    }
}