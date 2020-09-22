using TGS.Apis.BackendOffice.Services;
using TGS.Core.Models.Messages;

namespace TGS.Apis.BackendOffice.Controllers.Messages.Roles
{
    public class GetMenuResponse : MessageResponse
    {
        public MenuSetting[] Menus { get; set; }
    }
}
