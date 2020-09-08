using TGS.Apis.BackendOffice.Services;

namespace TGS.Apis.BackendOffice.Controllers.Messages.Validator.Role
{
    public class CreateRoleRequestValidator
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public MenuSetting[] Menus { get; set; }
    }
}
