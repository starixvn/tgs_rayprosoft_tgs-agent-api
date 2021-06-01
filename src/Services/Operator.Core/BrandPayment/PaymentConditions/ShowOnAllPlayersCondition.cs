using Tgs.Payment;

namespace TGS.Services.Operator.BrandPayment.PaymentConditions
{
    internal class ShowOnAllPlayersCondition : IBrandPaymentCondition
    {
        public bool IsValid(BrandPaymentInfo brandPayment, PlayerInfo player)
            => brandPayment.IsAllPlayers;
    }
}
