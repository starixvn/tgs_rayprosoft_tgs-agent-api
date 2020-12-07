using System;

namespace TGS.Services.BackOffice.Application.Models.BrandGameProvider
{
    public class BrandGameProviderModel
    {
        public string Name { get; set; }
        public DateTimeOffset From { get; set; }
        public DateTimeOffset To { get; set; }
    }
}