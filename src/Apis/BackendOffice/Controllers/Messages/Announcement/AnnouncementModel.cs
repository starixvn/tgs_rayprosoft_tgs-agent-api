using System;

namespace TGS.Apis.BackendOffice.Controllers.Messages.Announcement
{
    public class AnnouncementModel
    {
        public Guid Id { get; set; }
        public Guid BrandId { get; set; }
        public string BrandName { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public string Remarks { get; set; }
        public string CreatedByUsername { get; set; }
        public string VerifiedByUsername { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public DateTimeOffset From { get; set; }
        public DateTimeOffset To { get; set; }
        public int DisplayOrder { get; set; }
        public string Status { get; set; }
    }
}