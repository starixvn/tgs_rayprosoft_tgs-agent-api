using System;
using System.Collections.Generic;

namespace TGS.Apis.BackendOffice.DataService.Models
{
    public class Menu
    {
        public Guid Id { get; set; }

        public string Icon { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string Translation { get; set; }

        public ICollection<RoleMenu> RoleMenus { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public DateTimeOffset UpdatedOn { get; set; }

        public string CreatedByUsername { get; set; }

        public string UpdatedByUsername { get; set; }
    }
}
