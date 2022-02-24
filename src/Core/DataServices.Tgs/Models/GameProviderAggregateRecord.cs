using System;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class GameProviderAggregateRecord
    {
        public int Id { get; set; }

        public int AuditLogId { get; set; }

        public string Data { get; set; }

        public DateTimeOffset CreatedOn { get; set; }
    }
}