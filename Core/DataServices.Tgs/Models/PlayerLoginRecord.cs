using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class PlayerLoginRecord
    {
        public Guid Id { get; set; }
        public string PlayerCode { get; set; }
        public virtual Player Player { get; set; }
        public Guid BrandId { get; set; }
        public virtual Brand Brand { get; set; }
        public LoginStatus Status { get; set; }
        public PlatformType PlatformType { get; set; }
        public string IpAddress { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset ExpiredOn { get; set; }
    }
}