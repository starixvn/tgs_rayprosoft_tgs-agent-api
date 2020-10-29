using System;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;

namespace TGS.Apis.BackendOffice.Controllers.Messages.Announcement
{
    public class GetAnnouncementsRequest : PageableRequest
    {
        public AnnounceStatus[] Status { get; set; }
    }
}
