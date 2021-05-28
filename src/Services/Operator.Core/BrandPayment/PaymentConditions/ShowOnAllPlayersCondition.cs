using TGS.Services.Payment;
using TGS.Services.Operator.BrandPayment.PaymentConditions;
using TGS.Services.Operator;

namespace TGS.Services.Operator.BrandPayment.PaymentConditions
{
    internal class ShowOnAllPlayersCondition : IBrandPaymentCondition
    {
        public bool IsValid(BrandPaymentInfo brandPayment, PlayerInfo player)
            => brandPayment.IsAllPlayers;
    }
}
