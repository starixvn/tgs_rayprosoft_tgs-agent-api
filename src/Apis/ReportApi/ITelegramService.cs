using System.Threading.Tasks;

namespace TGS.Apis.ReportApi
{
    public interface ITelegramService
    {
        public Task SendMessage(string message);
    }
}