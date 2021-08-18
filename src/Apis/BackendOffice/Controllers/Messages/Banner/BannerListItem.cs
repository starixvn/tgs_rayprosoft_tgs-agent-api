using System;

namespace TGS.Apis.BackendOffice.Controllers.Messages.Banner
{
    public class BannerListItem
    {
        public DateTimeOffset BeginTime { get; set; }

        public DateTimeOffset? EndTime { get; set; }

        public string Modifier { get; set; }

        public int Status { get; set; }

        public string Title { get; set; }

        public DateTimeOffset UpdatedOn { get; set; }
    }
}
