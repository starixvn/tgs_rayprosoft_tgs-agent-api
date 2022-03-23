namespace TGS.Services.Promotion.Services
{
    public record ChangeOrderCommand
    {
        public ChangeOrderModel[] Orders { get; set; } = default!;
    }
}
