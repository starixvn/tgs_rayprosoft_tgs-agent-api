using System;
using System.Collections.Generic;
using TGS.Apis.BackendOffice.DataService.Enums;

namespace TGS.Apis.BackendOffice.DataService.Models
{
    // NB: Roles seeding uses explicit sql script to insert roles
    // because role Id uses Identity feature and EF prevents inserting it as a value
    // so if properties changed seeding script should be changed accordingly
    public class Role
    {
        public Guid Id { get; set; }

        public RoleType RoleType { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string Description { get; set; }

        public RoleStatus Status { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public DateTimeOffset UpdatedOn { get; set; }

        public string CreatedByUsername { get; set; }

        public string UpdatedByUsername { get; set; }

        public virtual ICollection<RoleMenu> Menus { get; set; }

        public ICollection<User> Users { get; set; }
    }
}
