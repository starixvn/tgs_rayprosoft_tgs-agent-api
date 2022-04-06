using System;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class PlayerBucketHistory
    {
        public int Id { get; set; }

        public Guid PlayerBucketId { get; set; }

        public string PlayerCode { get; set; }

        public decimal Turnover { get; set; }

        public DateTimeOffset From { get; set; }

        public DateTimeOffset To { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public string RoundData { get; set; }
    }
}