using System;

namespace TGS.Apis.TGS.Apis.BackendOffice.Controllers.Messages.UserAuthentication
{
    public class BaseRoleMenuEditModel
    {
        public Guid MenuId { get; set; }
        public bool Selected { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Translation { get; set; }
        public string UiSref { get; set; }
        public string UiSrefActive { get; set; }
        public int DisplayOrder { get; set; }
    }
}
