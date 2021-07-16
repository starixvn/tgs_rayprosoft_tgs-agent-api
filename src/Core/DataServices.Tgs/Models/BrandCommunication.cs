using System;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class BrandCommunication
    {
        public Guid Id { get; set; }
        public Guid BrandId { get; set; }
        public virtual Brand Brand { get; set; }
        public string Type { get; set; }
        public string Phone { get; set; }
        public string AppId { get; set; }
        public string QRCodeUrl { get; set; }
    }
}