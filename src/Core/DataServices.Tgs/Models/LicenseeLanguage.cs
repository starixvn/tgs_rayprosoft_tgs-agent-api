using System;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class LicenseeLanguage
    {
        public Guid LicenseeId { get; set; }

        public virtual Licensee Licensee { get; set; }

        public string CultureCode { get; set; }

        public virtual Language Language { get; set; }
    }
}