using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tgs.Payment.Clients;

namespace TGS.Services.BackOffice.Application.Models.Deposit
{
    public class ExportDepositModel
    {
        public int Sequence { get; init; }
        public string Username { get; init; }
        public string Id { get; init; }
        public string ThirdPartyOrderNo { get; init; }
        public string PaymentMethod { get; init; }
        public string PaymentName { get; init; }
        public decimal Amount { get; init; }
        public DateTimeOffset ApplicationTime { get; init; }
        public DateTimeOffset UpdateTime { get; init; }
        public string Status { get; init; }
        public string Remark { get; init; }
    }
}
