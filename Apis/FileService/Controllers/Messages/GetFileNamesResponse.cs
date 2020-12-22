using TGS.Core.Models.Messages;

namespace TGS.Apis.FileService.Controllers.Messages
{
    public class GetFileNamesResponse : MessageResponse
    {
        public string[] Names { get; set; }
    }
}