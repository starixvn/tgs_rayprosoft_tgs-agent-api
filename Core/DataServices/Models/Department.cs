using System;
using System.Collections.Generic;

namespace TGS.Core.DataServices.Models
{
    public class Department
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public DateTimeOffset UpdatedOn { get; set; }

        public string CreatedByUsername { get; set; }

        public string UpdatedByUsername { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}