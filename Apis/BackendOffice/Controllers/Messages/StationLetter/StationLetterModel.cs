using System;

namespace TGS.Apis.BackendOffice.Controllers.Messages.StationLetter
{
    public class StationLetterModel
    {
        public Guid Id { get; set; }
        public Guid BrandId { get; set; }
        public string BrandName { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public string Remarks { get; set; }
        public bool AssignedAllPlayers { get; set; }
        public string[] PlayerUsernames { get; set; }
        public string CreatedByUsername { get; set; }
        public string VerifiedByUsername { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public string Status { get; set; }
    }
}