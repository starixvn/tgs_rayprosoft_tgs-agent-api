using System;
using TGS.Core.DataServices.Enums;

namespace TGS.Core.DataServices.Models
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public TgsStatus Status { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public string CreatedByUsername { get; set; }
        public string UpdatedByUsername { get; set; }
    }
}
