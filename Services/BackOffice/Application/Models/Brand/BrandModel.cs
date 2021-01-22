using System;
using TGS.Core.Models.Enums;

namespace TGS.Services.BackOffice.Application.Models.Brand
{
    public class BrandModel
    {
        public Guid Id { get; set; }
        public Guid LicenseeId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public Status Status { get; set; }
        public string Description { get; set; }
        public string OperationalEmail { get; set; }
        public BrandGameProviderModel[] GameProviders { get; set; }
        public BrandCurrencyModel[] Currencies { get; set; }
        public BrandLanguageModel[] Languages { get; set; }
        public BrandBetLimitModel[] BetLimits { get; set; }
        public BrandGameModel[] Games { get; set; }
        public BrandGroupModel[] BrandGroups { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public string CreatedByUsername { get; set; }
        public string UpdatedByUsername { get; set; }
    }
}