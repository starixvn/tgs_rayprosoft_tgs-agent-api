using System;

namespace TGS.Apis.BackendOffice.Controllers.Messages.Announcement
{
    public class AnnouncementRequest
    {
        public string Subject { get; set; }
        public DateTimeOffset From { get; set; }
        public DateTimeOffset To { get; set; }
        public string Content { get; set; }
        public Guid BrandId { get; set; }
    }
}