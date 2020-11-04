using System;

namespace TGS.Core.DataServices.Models
{
    public class LicenseeLanguage
    {
        public Guid LicenseeId { get; set; }
        public Licensee Licensee { get; set; }
        public string CultureCode { get; set; }
        public Language Language { get; set; }
        public string LicenseeCultureCode { get; set; }
    }
}