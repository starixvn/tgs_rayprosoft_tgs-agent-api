namespace TGS.Services.Agent
{
    public interface IValidateCodeGenerator
    {
        string GenerateCode();

        string GenerateSecurityKey();
    }
}