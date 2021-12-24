using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGS.Core.Models.Enums;

namespace TGS.GameProviders.Repositories.Models
{
    public record PlayerLoginRecordModel
    {
        public Guid Id { get; set; }
        public string PlayerCode { get; set; }
        public Guid BrandId { get; set; }
        public LoginStatus Status { get; set; }
        public PlatformType PlatformType { get; set; }
        public string IpAddress { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset ExpiredOn { get; set; }
    }
}
