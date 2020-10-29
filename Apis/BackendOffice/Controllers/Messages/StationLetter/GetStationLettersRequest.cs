using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;

namespace TGS.Apis.BackendOffice.Controllers.Messages.StationLetter
{
    public class GetStationLettersRequest : PageableRequest
    {
        public StationLetterStatus Status { get; set; }
    }
}
