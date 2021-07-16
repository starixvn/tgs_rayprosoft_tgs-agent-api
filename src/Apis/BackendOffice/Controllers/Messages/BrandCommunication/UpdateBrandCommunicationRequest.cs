using TGS.Core.Models.Enums;

namespace TGS.Apis.BackendOffice.Controllers.Messages.BrandCommunication
{
    public class UpdateBrandCommunicationRequest
    {
        public CommunicationType Type { get; set; }
        public string Phone { get; set; }
        public string AppId { get; set; }
        public string QRCodeUrl { get; set; }
    }
}