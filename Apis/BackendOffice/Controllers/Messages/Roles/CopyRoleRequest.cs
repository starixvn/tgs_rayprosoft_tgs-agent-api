using FluentValidation.AspNetCore;
using TGS.Apis.BackendOffice.Controllers.Messages.Validator.Role;

namespace TGS.Apis.BackendOffice.Controllers.Messages.Roles
{

    public class CopyRoleRequest
    {
        public string Name { get; set; }
    }
}
