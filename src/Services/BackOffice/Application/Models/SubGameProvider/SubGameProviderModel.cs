using System;
using System.Text.Json;

namespace TGS.Services.BackOffice.Application.Models.SubGameProvider
{
    public class SubGameProviderModel
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
        public JsonDocument Localization { get; set; }
        public Guid GameProviderId { get; set; }

        public string ParentName { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public DateTimeOffset UpdatedOn { get; set; }

        public string CreatedByUsername { get; set; }

        public string UpdatedByUsername { get; set; }
    }
}