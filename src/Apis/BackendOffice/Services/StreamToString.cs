using System.IO;
using System.Threading.Tasks;

namespace TGS.Apis.BackendOffice.Services
{
    public interface IStreamToString
    {
        Task<string> GetStringAsync(Stream stream);
    }

    public class StreamToString : IStreamToString
    {
        async Task<string> IStreamToString.GetStringAsync(Stream stream)
        {
            var sr = new StreamReader(stream);
            return await sr.ReadToEndAsync();
        }
    }
}
