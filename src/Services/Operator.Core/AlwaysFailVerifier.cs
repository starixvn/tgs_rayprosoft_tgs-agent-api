namespace TGS.Services.Operator
{
    internal class AlwaysFailVerifier : IIdentityVerifier
    {
        public bool Verify(string id) => false;
    }
}