using TGS.Apis.BackendOffice.Services;

namespace TGS.Apis.BackendOffice.Controllers.Messages.Roles
{
    public class GetRoleResponse : MessageResponse
    {
        public MenuSetting[] MenuSettings { get; set; }
    }
}
