using System;
using TGS.Core.Models.Enums;

namespace TGS.Services.BackOffice.Application.Models.Brand
{
    public class BrandRequest : BaseRequest
    {
        public Guid Id { get; set; }
        public Guid LicenseeId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string OperationalEmail { get; set; }
        public string ApiKey { get; set; }
        public string ClientId { get; set; }
        public string SecretKey { get; set; }
        public int? SettlementDay { get; set; }
        public SettlementIntervalType SettlementIntervalType { get; set; }
        public BrandGameProviderModel[] GameProviders { get; set; }
        public string[] CurrencyCodes { get; set; }
        public BrandLanguageModel[] Languages { get; set; }
        public BrandBetLimitModel[] BetLimits { get; set; }
        public string[] Domains { get; set; }
        public decimal PlatformCharge { get; set; }
        public bool CheckDividendActiveCount { get; set; }
    }
}