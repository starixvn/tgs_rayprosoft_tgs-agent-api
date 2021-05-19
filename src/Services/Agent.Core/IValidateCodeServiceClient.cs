using System.Threading.Tasks;

namespace TGS.Services.Agent
{
    public interface IValidateCodeServiceClient
    {
        ValueTask<bool> SaveValidateCodeDataAsync(string validateCode, string token, string content);

        ValueTask<string?> GetValidateCodeDataAsync(string validateCode, string token);
    }
}