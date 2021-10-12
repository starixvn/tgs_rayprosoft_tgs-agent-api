using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.Models.Messages;

namespace TGS.Apis.BackendOffice.Controllers.Messages.Withdraw
{
    public class WithdrawViewModel
    {
        public PagedResponse<WithdrawFormViewModel>? Datas { get; init; }

        public decimal Sum { get; init; }
    }
}
