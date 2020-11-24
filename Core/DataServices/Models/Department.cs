using System;
using System.Collections.Generic;

namespace TGS.Core.DataServices.Models
{
    // NB: Roles seeding uses explicit sql script to insert roles
    // because role Id uses Identity feature and EF prevents inserting it as a value
    // so if properties changed seeding script should be changed accordingly
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