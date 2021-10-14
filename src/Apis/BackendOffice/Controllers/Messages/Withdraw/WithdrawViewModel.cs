using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.Models.Messages;

namespace TGS.Apis.BackendOffice.Controllers.Messages.Withdraw
{
    public class WithdrawViewModel : PagedResponse<WithdrawFormViewModel>
    {
        public decimal Sum { get; init; }
    }
}
