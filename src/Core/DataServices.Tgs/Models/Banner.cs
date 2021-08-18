using System;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class Banner
    {
        public DateTimeOffset BeginTime { get; set; }

        public Guid BrandId { get; set; }

        public string CreatedByUserName { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public DateTimeOffset? EndTime { get; set; }

        public Guid Id { get; set; }

        public bool IsEnabled { get; set; }

        public string Localization { get; set; }

        public string Remark { get; set; }

        public int Status { get; set; }

        public string Title { get; set; }

        public string UpdatedByUserName { get; set; }

        public DateTimeOffset UpdatedOn { get; set; }
    }
}
