using System;
using System.Threading.Tasks;

namespace TGS.Services.Operator
{
    public interface IValidateCodeOperator
    {
        ValueTask SaveValidateDataAsync(string validateCode, ValidateCodeData data);

        ValueTask SendMessageAsync(string phone, string shortMessage, string currency, Guid brandId);
    }
}
