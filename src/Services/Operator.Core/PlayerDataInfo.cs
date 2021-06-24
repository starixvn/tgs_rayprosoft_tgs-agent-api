namespace TGS.Services.Operator
{
    public record PlayerDataInfo(
        string Username,
        string RealName,
        string Phone,
        string Email,
        string QQ,
        string Wechat,
        bool IsSetPayPassword,
        bool IdentityAuthentication
    );
}