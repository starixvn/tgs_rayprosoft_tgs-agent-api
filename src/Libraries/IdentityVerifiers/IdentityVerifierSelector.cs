namespace TGS.IdentityVerifiers
{
    public class IdentityVerifierSelector
    {
        public IIdentityVerifier GetIdentityVerifierByCurrency(string currency) => currency switch
        {
            "CNY" => new ChineseIdentityVerifier(),
            _ => new SoutheastAsiaIdentityVerifier()
        };
    }
}
