//using System;
//using System.Threading;
//using System.Threading.Tasks;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using NATS.Client.Rx.Ops;
//using TGS.Apis.FileService.DataService.Services;
//using TGS.Core.Infrastructures.Nats;
//using TGS.Core.Infrastructures.Nats.Model;

//namespace TGS.Apis.FileService
//{
//    /// <summary>
//    /// Nats MessageHandler BackgroundService
//    /// </summary>
//    /// <seealso cref="Microsoft.Extensions.Hosting.BackgroundService" />
//    public class NatsMessageService : BackgroundService
//    {
//        private readonly INatsManager _natsManager;
//        private readonly IServiceProvider _serviceProvider;

//        public NatsMessageService(INatsManager natsManager, IServiceProvider serviceProvider)
//        {
//            _natsManager = natsManager ?? throw new ArgumentNullException(nameof(natsManager));
//            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
//        }

//        /// <summary>
//        /// This method is called when the <see cref="T:Microsoft.Extensions.Hosting.IHostedService" /> starts. The implementation should return a task that represents
//        /// the lifetime of the long running operation(s) being performed.
//        /// </summary>
//        /// <param name="stoppingToken">Triggered when <see cref="M:Microsoft.Extensions.Hosting.IHostedService.StopAsync(System.Threading.CancellationToken)" /> is called.</param>
//        /// <returns>
//        /// A <see cref="T:System.Threading.Tasks.Task" /> that represents the long running operations.
//        /// </returns>
//        /// <exception cref="NotImplementedException"></exception>
//        protected override Task ExecuteAsync(CancellationToken stoppingToken)
//        {
//            var observer = _natsManager.SubscribeWithObserveAsync(nameof(ObjectFileEventModel));

//            var sub = observer
//                .Select((msg) =>
//                {
//                    var fileObject = _natsManager.Deserialize<ObjectFileEventModel>(msg.Data);

//                    using var scope = _serviceProvider.CreateScope();

//                    var service = scope.ServiceProvider.GetRequiredService<IUploadService>();

//                    return service.AddStationLetterFileAsync(fileObject);
//                })
//                .Subscribe();

//            stoppingToken.Register(() =>
//            {
//                sub.Dispose();
//            });

//            return Task.CompletedTask;
//        }
//    }
//}

