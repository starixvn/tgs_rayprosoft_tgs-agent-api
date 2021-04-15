using System.Threading.Tasks;

namespace TGS.Services.Operator
{
    internal class IdentityCondition : IBrandPaymentCondition
    {
        public ValueTask<bool> IsValidAsync(PlayerInfo player)
            => ValueTask.FromResult(player.IdentityAuthentication);
    }
}
