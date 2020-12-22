using TGS.Core.Models.Messages;

namespace TGS.Apis.BackendOffice.Controllers.Messages.Roles
{
    public class GetRolesRequest : PageableRequest
    {
        public bool? IsRoot { get; set; }
        public string Name { get; set; }
    }
}