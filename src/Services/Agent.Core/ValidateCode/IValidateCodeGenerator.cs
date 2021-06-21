using TGS.Services.Agent;
namespace TGS.Services.Agent.ValidateCode
{
    public interface IValidateCodeGenerator
    {
        string GenerateCode();

        string GenerateSecurityKey();
    }
}
