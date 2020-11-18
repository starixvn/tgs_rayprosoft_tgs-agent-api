using System.Threading.Tasks;
using FileService.DataService;
using FileService.DataService.Model;
using NATS.Client.Rx.Ops;
using TGS.Core.Infrastructures.Nats;
using TGS.Core.Infrastructures.Nats.Model;

namespace FileService.Services
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