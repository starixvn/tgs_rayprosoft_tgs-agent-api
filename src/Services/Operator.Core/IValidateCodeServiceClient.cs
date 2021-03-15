using System.Threading.Tasks;

namespace TGS.Services.Operator
{
    public interface IValidateCodeServiceClient
    {
        ValueTask<bool> SaveValidateCodeDataAsync(string validateCode, string token, string content);

        ValueTask<string?> GetValidateCodeDataAsync(string validateCode, string token);
    }
}