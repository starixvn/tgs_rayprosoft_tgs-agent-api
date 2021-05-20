using System.Globalization;
using System.Threading.Tasks;

namespace TGS.Services.Agent
{
    public interface IValidateCodeOperator
    {
        ValueTask<bool> SaveValidateDataAsync<TData>(string validateCode, string token, TData data);

        ValueTask<TData?> GetValidateCodeDataAsync<TData>(string validateCode, string token);

        ValueTask SendEmailAsync(CultureInfo cultureInfo, string email, string validateCode);
    }
}