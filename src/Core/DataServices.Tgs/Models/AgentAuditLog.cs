using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class AgentAuditLog
    {
        public Guid Id { get; set; }
        public Guid AgentId { get; set; }
        public virtual Agent Agent { get; set; }
        public AgentOperation Operation { get; set; }
        public string CreatedByUsername { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
    }
}