using System.IO;
using System.Threading.Tasks;

namespace TGS.Apis.BackendOffice.Services
{
    public interface IStreamToString
    {
        Task<string> GetString(Stream stream);
    }
    public class StreamToString : IStreamToString
    {
       async Task<string> IStreamToString.GetString(Stream stream)
        {
            var sr = new StreamReader(stream);
            return await sr.ReadToEndAsync();
        }
    }
}
