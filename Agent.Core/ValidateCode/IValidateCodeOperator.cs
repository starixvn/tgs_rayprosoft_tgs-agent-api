using System.Globalization;
using System.Threading.Tasks;

namespace TGS.Services.Agent.ValidateCode;

public interface IValidateCodeOperator
{
    ValueTask<TData?> GetValidateCodeDataAsync<TData>(string validateCode, string token);

    ValueTask<bool> SaveValidateDataAsync<TData>(string validateCode, string token, TData data);

    ValueTask SendEmailAsync(CultureInfo cultureInfo, string email, string validateCode);
}