using TGS.Services.Promotion.Services;

namespace TGS.Services.Promotion.Models.Promotion
{
    public class ChangeOrderRequest
    {
        public ChangeOrderModel[] Orders { get; set; } = default!;
    }
}
