using System;
using System.Globalization;
using System.Threading.Tasks;

namespace TGS.Services.Operator
{
    public interface IValidateCodeOperator
    {
        ValueTask<bool> SaveValidateDataAsync<TData>(string validateCode, string token, TData data);

        ValueTask SendMessageAsync(string phone, string shortMessage, string currency, Guid brandId);

        ValueTask<TData?> GetValidateCodeDataAsync<TData>(string validateCode, string token);

        ValueTask SendEmailAsync(CultureInfo cultureInfo, string email, string ValidateCode);
    }
}