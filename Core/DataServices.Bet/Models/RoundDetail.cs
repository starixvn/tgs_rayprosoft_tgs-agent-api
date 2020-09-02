using System;

namespace TGS.Core.DataServices.Bet.Models
{
    public class RoundDetail
    {
        public Guid RoundId { get; set; }
        public Round Round { get; set; }
        public string AdditionalData { get; set; }
    }
}
