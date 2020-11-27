using TGS.Core.Models.Messages;

namespace TGS.Apis.FileService.Controllers.Messages
{
    public class UploadResponse : MessageResponse
    {
        public string Location { get; set; }
    }
}