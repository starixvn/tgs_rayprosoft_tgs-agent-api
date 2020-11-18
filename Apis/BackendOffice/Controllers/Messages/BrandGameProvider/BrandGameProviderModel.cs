using System;

namespace TGS.Apis.BackendOffice.Controllers.Messages.BrandGameProvider
{
    public class BrandGameProviderModel
    {
        public string Name { get; set; }
        public DateTimeOffset From { get; set; }
        public DateTimeOffset To { get; set; }
    }
}