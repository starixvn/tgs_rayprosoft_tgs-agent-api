using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class Announcement
    {
        public virtual Brand Brand { get; set; }

        public Guid BrandId { get; set; }

        public string Content { get; set; }

        public string CreatedByUsername { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public int DisplayOrder { get; set; }

        public DateTimeOffset From { get; set; }

        public Guid Id { get; set; }

        public string Localization { get; set; }

        public string Remarks { get; set; }

        public AnnounceStatus Status { get; set; }

        public string Subject { get; set; }

        public DateTimeOffset To { get; set; }

        public string UpdatedByUsername { get; set; }

        public DateTimeOffset UpdatedOn { get; set; }

        public string VerifiedByUsername { get; set; }
    }
}
