namespace TGS.Services.Operator
{
    public interface IIdentityVerifier
    {
        bool Verify(string id);
    }
}