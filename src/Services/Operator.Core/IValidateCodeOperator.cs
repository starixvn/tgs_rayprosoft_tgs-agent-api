using System;
using System.Threading.Tasks;

namespace TGS.Services.Operator
{
    public interface IValidateCodeOperator
    {
        ValueTask<bool> SaveValidateDataAsync(string validateCode, ValidateCodeData data);

        ValueTask SendMessageAsync(string phone, string shortMessage, string currency, Guid brandId);

        ValueTask<ValidateCodeData?> GetValidateCodeDataAsync(string validateCode, string token);
    }
}
