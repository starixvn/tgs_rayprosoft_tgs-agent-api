using System;
using TGS.Core.Models.Enums;

namespace TGS.Apis.BackendOffice.Controllers.Messages.BrandCommunication
{
    public class CreateBrandCommunicationRequest
    {
        public Guid BrandId { get; set; }
        public CommunicationType Type { get; set; }
        public string Phone { get; set; }
        public string AppId { get; set; }
        public string QRCodeUrl { get; set; }
    }
}