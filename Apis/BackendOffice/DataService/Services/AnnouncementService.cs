using System;
using System.Threading.Tasks;
using TGS.Apis.BackendOffice.Resources;
using TGS.Core.DataServices;

namespace TGS.Apis.BackendOffice.DataService.Services
{
    public interface IAnnouncementService
    {
        Task CreateAnnouncementAsync(Announcement announcement);
        Task UpdateAnnouncementAsync(Announcement announcement);
    }

    public class AnnouncementService : IAnnouncementService
    {
        private readonly ITgsRepository _repository;

        public AnnouncementService(ITgsRepository repository)
        {
            _repository = repository;
        }

        Task IAnnouncementService.CreateAnnouncementAsync(Announcement announcement)
        {
            //TODO: send a request to file service by nats
            throw new NotImplementedException();
        }

        Task IAnnouncementService.UpdateAnnouncementAsync(Announcement announcement)
        {
            //TODO: send a request to file service by nats
            throw new NotImplementedException();
        }
    }
}