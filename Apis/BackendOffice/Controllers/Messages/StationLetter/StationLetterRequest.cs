using System;

namespace TGS.Apis.BackendOffice.Controllers.Messages.StationLetter
{
    public class StationLetterRequest
    {
        public Guid BrandId { get; set; }
        public string Subject { get; set; }
        public DateTimeOffset From { get; set; }
        public DateTimeOffset To { get; set; }
        public string Content { get; set; }
        public bool AssignedAllPlayers { get; set; }
        public string[] PlayerUsernames { get; set; }
    }
}