using System.Threading.Tasks;

namespace TGS.Services.Operator
{
    public interface IValidateCodeOperator
    {
        ValueTask SaveValidateDataAsync(string validateCode, ValidateCodeData data);

        Task SendMessageAsync(string phone, string shortMessage);
    }
}
