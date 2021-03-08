using System.Threading.Tasks;

namespace TGS.Services.ValidateCode
{
    public interface IValidateCodeStore
    {
        ValueTask SaveDataAsync(string validateCode, string content);

        ValueTask<string> GetDataAsync(string validateCode);
    }
}
