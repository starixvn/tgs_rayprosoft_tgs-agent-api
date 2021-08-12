using TGS.Shared.Game.Models.WGHorse.Enums;

namespace TGS.Shared.Game.Models.WGHorse.Outgoing
{
    public record WGHorseAccessMoneyRequest
    {
        public string PlayerCode { get; init; }
        public AmountTypes AmountType { get; init; }
        public string OrderId { get; init; }
        public string Amount { get; init; }
    }
}
