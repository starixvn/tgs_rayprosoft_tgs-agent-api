using System.Data;
using System.Threading.Tasks;
using TGS.Apis.BackendOffice.Resources;
using TGS.Core.DataServices;

namespace TGS.Apis.BackendOffice.DataService.Services
{
    public interface IAnnouncementService
    {
        Task CreateAnnouncement(Announcement announcement);
    }

    public class AnnouncementService : IAnnouncementService
    {
        private readonly ITgsRepository _repository;

        public AnnouncementService(ITgsRepository repository)
        {
            _repository = repository;
        }

        async Task IAnnouncementService.CreateAnnouncement(Announcement announcement)
        {
            await using var tran = await _repository.BeginTransactionAsync(IsolationLevel.ReadCommitted);
        }
    }
}