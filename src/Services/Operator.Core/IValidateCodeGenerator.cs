namespace TGS.Services.Operator
{
    public interface IValidateCodeGenerator
    {
        string GenerateCode();

        string GenerateToken();
    }
}
