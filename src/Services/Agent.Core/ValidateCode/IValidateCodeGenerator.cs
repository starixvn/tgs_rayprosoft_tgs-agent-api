namespace TGS.Services.Agent.ValidateCode
{
    public interface IValidateCodeGenerator
    {
        string GenerateCode();

        string GenerateSecurityKey();
    }
}
