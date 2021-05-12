using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class AgentLoginRecord
    {
        public Guid Id { get; set; }
        public Guid AgentId { get; set; }
        public virtual Agent Agent { get; set; }
        public Guid BrandId { get; set; }
        public virtual Brand Brand { get; set; }
        public LoginStatus Status { get; set; }
        public string IpAddress { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset ExpiredOn { get; set; }
    }
}