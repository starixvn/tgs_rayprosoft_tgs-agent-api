using System;

namespace TGS.Services.BackOffice.Application.Queries.Deposit
{
    public class DepositFormQuery
    {
        public Guid BrandId { get; set; }

        public string UserName { get; set; }

        public string ChannelName { get; set; }

        public Guid? PaymentMethodId { get; set; }

        public int? Status { get; set; }

        public (DateTimeOffset? Begin, DateTimeOffset? End)? CreateOnRange { get; set; }

        public (DateTimeOffset? Begin, DateTimeOffset? End)? UpdateOnRange { get; set; }
    }
}
