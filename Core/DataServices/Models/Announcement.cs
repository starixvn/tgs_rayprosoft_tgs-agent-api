using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.Models
{
    public class Announcement
    {
        public Guid Id { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public string Description { get; set; }
        public AnnounceStatus Status { get; set; }
        public DateTimeOffset From { get; set; }
        public DateTimeOffset To { get; set; }
        public int DisplayerOrder { get; set; }
        public string VerifiedByUsername { get; set; }
        public string CreatedByUsername { get; set; }
        public string UpdatedByUsername { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
    }
}
