using System;
using TGS.Core.Models.Enums;

namespace TGS.Apis.BackendOffice.Controllers.Messages.Reseller
{
    public class ResellerModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public Status Status { get; set; }
        public string Description { get; set; }
        public string OperationalEmail { get; set; }
        public ResellerGameProviderModel[] ResellerGameProviders { get; set; }
        public ResellerCurrencyModel[] ResellerCurrencies { get; set; }
        public ResellerLanguageModel[] ResellerLanguages { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public string CreatedByUsername { get; set; }
        public string UpdatedByUsername { get; set; }
    }
}