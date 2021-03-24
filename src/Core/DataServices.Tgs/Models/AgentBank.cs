using System;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class AgentBank
    {
        public Guid Id { get; set; }
        public string RealName { get; set; }
        public string BankName { get; set; }
        public string BankAccount { get; set; }
        public Guid AgentId { get; set; }
        public virtual Agent Agent { get; set; }
        public string CreatedByUsername { get; set; }
        public string UpdatedByUsername { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
    }
}