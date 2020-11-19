using System.Threading.Tasks;
using NATS.Client.Rx.Ops;
using TGS.Apis.FileService.DataService;
using TGS.Apis.FileService.DataService.Model;
using TGS.Core.Infrastructures.Nats;
using TGS.Core.Infrastructures.Nats.Model;

namespace TGS.Apis.FileService.Services
{
    public interface INatsMessageHandler
    {
        Task SubscribeAsync();
    }

    public class NatsMessageHandler : INatsMessageHandler
    {
        private readonly INatsManager _natsManager;
        private readonly IRepository<StationLetterFile> _repository;

        public NatsMessageHandler(INatsManager natsManager, IRepository<StationLetterFile> repository)
        {
            _natsManager = natsManager;
            _repository = repository;
        }

        Task INatsMessageHandler.SubscribeAsync()
        {
            return Task.FromResult(_natsManager.SubscribeWithObserveAsync(nameof(ObjectFileEventModel)).Subscribe(x =>
            {
                var fileObject = _natsManager.Deserialize<ObjectFileEventModel>(x.Data);

                var stationLetterFile = new StationLetterFile
                {
                    Id = fileObject.Id,
                    Files = fileObject.FileNames
                };

                _repository.InsertAsync(stationLetterFile);
            }));
        }
    }
}