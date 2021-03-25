using System;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class PlayerAuditLog
    {
        public Guid Id { get; set; }
        public string ModuleName { get; set; }
        public string Username { get; set; }
        public string Operation { get; set; }
        public string CreatedByUsername { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
    }
}