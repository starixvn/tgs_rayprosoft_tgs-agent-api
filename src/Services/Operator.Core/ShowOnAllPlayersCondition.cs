using System.Threading.Tasks;

namespace TGS.Services.Operator
{
    internal class ShowOnAllPlayersCondition : IBrandPaymentCondition
    {
        public bool IsValid(BrandPaymentInfo brandPayment, PlayerInfo player)
            => brandPayment.IsAllPlayers;
    }
}
