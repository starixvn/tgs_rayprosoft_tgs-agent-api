using System;
using System.Collections.Generic;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class StationLetter
    {
        public bool AssignedAllPlayers { get; set; }

        public virtual Brand Brand { get; set; }

        public Guid BrandId { get; set; }

        public string Content { get; set; }

        public string CreatedByUsername { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public Guid Id { get; set; }

        public string Localization { get; set; }

        public virtual ICollection<StationLetterPlayer> Players { get; set; }

        public string Remarks { get; set; }

        public StationLetterStatus Status { get; set; }

        public string Subject { get; set; }

        public string UpdatedByUsername { get; set; }

        public DateTimeOffset UpdatedOn { get; set; }

        public string VerifiedByUsername { get; set; }
    }
}
