using Tgs.Payment.Clients;

namespace TGS.Services.Operator.BrandPayment.PaymentConditions;

internal class ShowOnAllPlayersCondition : IBrandPaymentCondition
{
    public bool IsValid(BrandPaymentInfo brandPayment, Tgs.PlayerService.Client.PlayerInfo player)
        => brandPayment.IsAllPlayers;
}
