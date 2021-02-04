using System;
using System.Collections.Generic;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class GameActionBatch
    {
        public Guid Id { get; set; }
        public string ExternalId { get; set; }
        public TgsApiError ErrorType { get; set; }
        public string ErrorDescription { get; set; }
        public int TransactionsCount { get; set; }
        public string ErrorsJson { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public Guid GameProviderConfigurationId { get; set; }
        public virtual GameProviderConfiguration GameProviderConfiguration { get; set; }
        public virtual ICollection<GameAction> GameActions { get; set; }
    }
}