using System;

namespace TGS.Core.DataServices.Models
{
    public class Setting
    {
        public string Key { get; set; }

        public string Value { get; set; }

        public string Description { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public DateTimeOffset UpdatedOn { get; set; }

        public string CreatedByUsername { get; set; }

        public string UpdatedByUsername { get; set; }
    }
}