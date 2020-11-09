using TGS.Core.Models.Messages;

namespace TGS.Apis.BackendOffice.Controllers.Messages.Payment
{
    public class UploadFileResponse : MessageResponse
    {
        public string[] Players { get; set; }
        public string[] NotExistsPlayers { get; set; }
    }
}