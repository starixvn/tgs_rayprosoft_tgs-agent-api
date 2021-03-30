using System.Threading.Tasks;

namespace TGS.Services.Operator
{
    public interface IWalletRepository
    {
        ValueTask CreateDepositFormAsync(BankDepositForm form);

        ValueTask<BalanceInfo> GetBalanceInfoAsync(string playerCode);
    }
}
