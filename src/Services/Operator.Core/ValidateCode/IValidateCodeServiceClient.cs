using System.Threading.Tasks;
using TGS.Services.Operator;

namespace TGS.Services.Operator.ValidateCode
{
    public interface IValidateCodeServiceClient
    {
        ValueTask<bool> SaveValidateCodeDataAsync(string validateCode, string token, string content);

        ValueTask<string?> GetValidateCodeDataAsync(string validateCode, string token);
    }
}
