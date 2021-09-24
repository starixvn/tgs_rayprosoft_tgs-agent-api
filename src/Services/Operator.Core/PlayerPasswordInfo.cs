namespace TGS.Services.Operator
{
    public record PlayerPasswordInfo
    (
        string Username,
        string PlayerCode,
        byte[] Salt,
        byte[] Password
    );
}