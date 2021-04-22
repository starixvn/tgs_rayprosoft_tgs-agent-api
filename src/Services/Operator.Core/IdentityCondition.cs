namespace TGS.Services.Operator
{
    internal class IdentityCondition : IBrandPaymentCondition
    {
        private readonly bool _enabled;

        public IdentityCondition(bool enabled)
        {
            _enabled = enabled;
        }

        public bool IsValid(BrandPaymentInfo brandPayment, PlayerInfo player)
            => _enabled ? player.IdentityAuthentication : true;
    }
}
