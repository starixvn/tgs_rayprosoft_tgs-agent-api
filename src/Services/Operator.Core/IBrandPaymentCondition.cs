using System.Threading.Tasks;

namespace TGS.Services.Operator
{
    public interface IBrandPaymentCondition
    {
        ValueTask<bool> IsValidAsync(PlayerInfo player);
    }
}
