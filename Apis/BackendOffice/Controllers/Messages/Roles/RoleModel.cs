using System;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services;

namespace TGS.Apis.BackendOffice.Controllers.Messages.Roles
{
    public class RoleModel
    {
        public Guid Id { get; set; }
        public RoleType RoleType { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public bool IsRoot { get; set; }
        public MenuSetting[] Menus { get; set; }
        public string CreatedByUsername { get; set; }
        public string UpdatedByUsername { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
    }
}