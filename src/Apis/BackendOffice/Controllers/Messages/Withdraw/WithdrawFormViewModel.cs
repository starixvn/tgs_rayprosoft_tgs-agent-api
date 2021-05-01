using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TGS.Apis.BackendOffice.Controllers.Messages.Deposit;

namespace TGS.Apis.BackendOffice.Controllers.Messages.Withdraw
{
    public class WithdrawFormViewModel
    {
        public Guid Id { get; internal set; }
        public string PlayerCode { get; internal set; }
        public string UserName { get; internal set; }
        public Guid BrandId { get; internal set; }
        public Guid? BrandPaymentId { get; internal set; }
        public Guid? PaymentMethodId { get; internal set; }
        public string ChannelName { get; internal set; }
        public FlowStatus Status { get; internal set; }
        public decimal Amount { get; internal set; }
        public string Remark { get; internal set; }
        public DateTimeOffset CreatedOn { get; internal set; }
        public DateTimeOffset UpdatedOn { get; internal set; }
    }
}
