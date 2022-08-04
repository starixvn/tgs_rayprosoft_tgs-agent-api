using Tgs.Payment.Clients;

namespace TGS.Services.BackOffice.BrandPayment.PaymentConditions;

internal class ShowOnAllPlayersCondition : IBrandPaymentCondition
{
    public bool IsValid(BrandPaymentInfo brandPayment, Tgs.PlayerService.Client.PlayerInfo player, decimal depositedAmount)
        => brandPayment.IsAllPlayers;
}
