using System.Threading.Tasks;

namespace TGS.Apis.FileService.DataService.Services
{
    public interface IPromotionService
    {
        Task AddPromotionFileAsync(string id, string[] fileNames);
    }
}