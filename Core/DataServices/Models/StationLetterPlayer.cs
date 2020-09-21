using System;

namespace TGS.Core.DataServices.Models
{
    public class StationLetterPlayer
    {
        public Guid StationLetterId { get; set; }
        public string PlayerCode { get; set; }
        public bool IsRead { get; set; }
        public DateTimeOffset? ReadOn { get; set; }
        public StationLetter StationLetter { get; set; }
        public Player Player { get; set; }
    }
}
