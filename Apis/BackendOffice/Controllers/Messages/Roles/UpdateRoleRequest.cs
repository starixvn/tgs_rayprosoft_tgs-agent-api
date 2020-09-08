using TGS.Apis.BackendOffice.Services;

namespace TGS.Apis.BackendOffice.Controllers.Messages.Roles
{
    public class UpdateRoleRequest
    {
        public string Name { get; set; }
        public MenuSetting[] Menus { get; set; }
    }
}
