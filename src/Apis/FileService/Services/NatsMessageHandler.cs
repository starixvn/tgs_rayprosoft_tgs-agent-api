//using System.Threading.Tasks;
//using NATS.Client.Rx.Ops;
//using TGS.Apis.FileService.DataService.Services;
//using TGS.Core.Infrastructures.Nats;
//using TGS.Core.Infrastructures.Nats.Model;

//namespace TGS.Apis.FileService.Services
//{
//    public interface INatsMessageHandler
//    {
//        Task SubscribeAsync();
//    }

//    public class NatsMessageHandler : INatsMessageHandler
//    {
//        private readonly INatsManager _natsManager;
//        private readonly IUploadService _service;

//        public NatsMessageHandler(INatsManager natsManager, IUploadService service)
//        {
//            _natsManager = natsManager;
//            _service = service;
//        }

//        public Task SubscribeAsync()
//        {
//            var observer = _natsManager.SubscribeWithObserveAsync(nameof(ObjectFileEventModel));
//            return Task.FromResult(_natsManager.SubscribeWithObserveAsync(nameof(ObjectFileEventModel)).Subscribe(
//                x =>
//                {
//                    var fileObject = _natsManager.Deserialize<ObjectFileEventModel>(x.Data);
//                    _ = _service.AddStationLetterFileAsync(fileObject);
//                }));
//        }
//    }
//}

