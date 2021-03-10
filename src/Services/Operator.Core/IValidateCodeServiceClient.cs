using System.Threading.Tasks;

namespace TGS.Services.Operator
{
    public interface IValidateCodeServiceClient
    {
        ValueTask SaveValidateCodeDataAsync(string validateCode, string content);
    }
}