using System.Threading.Tasks;

namespace TGS.Apis.FileService.DataService.Services
{
    public interface IStationLetterService
    {
        Task AddStationLetterFileAsync(string id, string[] fileNames);
    }
}