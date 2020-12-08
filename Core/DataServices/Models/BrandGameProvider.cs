using System;
using System.Collections.Generic;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.Models
{
    public class BrandGameProvider
    {
        public Guid Id { get; set; }
        public Guid BrandId { get; set; }

        public virtual Brand Brand { get; set; }

        public Guid GameProviderId { get; set; }

        public virtual GameProvider GameProvider { get; set; }

        public Status Status { get; set; }

        public Guid GameProviderConfigurationId { get; set; }

        public virtual GameProviderConfiguration GameProviderConfiguration { get; set; }

        public string DefaultLanguageCode { get; set; }

        public virtual Language DefaultLanguage { get; set; }

        public string DesktopQueryString { get; set; }

        public string MobileQueryString { get; set; }

        public virtual ICollection<BrandGameProviderSchedule> Schedules { get; set; }
    }
}