using System.Security.Cryptography;
using System.Text;

namespace TGS.Core.Infrastructures.Security
{
    public interface ISymmetricCryptoProvider
    {
        string EncryptString(string original, string iv, string key, CipherMode mode = CipherMode.CBC, PaddingMode padding = PaddingMode.PKCS7, Encoding encoding = null);

        string DecryptString(string encrypted, string iv, string key, CipherMode mode = CipherMode.CBC, PaddingMode padding = PaddingMode.PKCS7, Encoding encoding = null);
    }
}
