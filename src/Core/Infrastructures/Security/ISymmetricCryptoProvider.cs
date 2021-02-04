using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TGS.Core.Infrastructures.Security
{
    public interface ISymmetricCryptoProvider
    {
        Task<string> EncryptStringAsync(string original, string iv, string key, CipherMode mode = CipherMode.CBC,
            PaddingMode padding = PaddingMode.PKCS7, Encoding encoding = null);

        Task<string> DecryptStringAsync(string encrypted, string iv, string key, CipherMode mode = CipherMode.CBC,
            PaddingMode padding = PaddingMode.PKCS7, Encoding encoding = null);
    }
}