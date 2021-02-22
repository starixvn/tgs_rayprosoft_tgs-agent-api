using System;
using System.Collections.Generic;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class GameActionBatch
    {
        public Guid Id { get; set; }
        public string ExternalId { get; set; }
        public TgsApiError Error { get; set; }
        public string ErrorMessage { get; set; }
        public int TransactionsCount { get; set; }
        public string ErrorsJson { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public Guid GameProviderId { get; set; }
        public virtual GameProvider GameProvider { get; set; }
        public virtual ICollection<GameAction> GameActions { get; set; }
    }
}