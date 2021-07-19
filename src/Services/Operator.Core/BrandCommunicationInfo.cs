using System;

namespace TGS.Services.Operator
{
    public class BrandCommunicationInfo
    {
        public Guid Id { get; set; }
        public string CommunicationType { get; set; } = default!;
        public string Phone { get; set; } = default!;
        public string AppId { get; set; } = default!;
        public string QRCodeUrl { get; set; } = default!;
    }
}