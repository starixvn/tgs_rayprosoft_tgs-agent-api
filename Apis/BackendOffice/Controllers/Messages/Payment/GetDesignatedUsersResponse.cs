using TGS.Core.Models.Messages;

namespace TGS.Apis.BackendOffice.Controllers.Messages.Payment
{
    public class GetDesignatedUsersResponse : MessageResponse
    {
        public string[] Players { get; set; }
    }
}