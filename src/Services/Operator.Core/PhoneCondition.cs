using System.Threading.Tasks;

namespace TGS.Services.Operator
{
    internal class PhoneCondition : IBrandPaymentCondition
    {
        public ValueTask<bool> IsValidAsync(PlayerInfo player)
            => ValueTask.FromResult(player.PhoneAuthentication);
    }
}
