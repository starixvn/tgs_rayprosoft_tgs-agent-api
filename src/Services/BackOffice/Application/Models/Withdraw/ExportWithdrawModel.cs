using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.Services.BackOffice.Application.Models.Withdraw
{
    public class ExportWithdrawModel
    {
        public int Sequence { get; init; }
        public string Username { get; init; }
        public string UseType { get; init; }
        public string Id { get; init; }
        public string ThirdPartyOrderNo { get; init; }
        public decimal Amount { get; init; }
        public DateTimeOffset ApplicationTime { get; init; }
        public DateTimeOffset UpdateTime { get; init; }
        public string Status { get; init; }
        public string Remark { get; init; }
    }
}
