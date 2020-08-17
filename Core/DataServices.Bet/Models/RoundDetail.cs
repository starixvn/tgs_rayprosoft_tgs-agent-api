using System;

namespace TGS.Core.DataServices.Bet.Models
{
    public class RoundDetail
    {
        public Guid Id { get; set; }
        public Guid RoundId { get; set; }
        public Round Round { get; set; }
        public string AdditionalData { get; set; }
    }
}
