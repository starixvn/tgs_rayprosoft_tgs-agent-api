using System;
using Microsoft.Extensions.Logging;
using TGS.Apis.LogApi.Interfaces;
using TGS.Apis.LogApi.Loggers;
using TGS.Infrastructures.Log.Events;
using TGS.Infrastructures.Unity;
using Unity;

namespace TGS.Apis.LogApi.Services
{
    public sealed class LogApiFactory : BaseFactory
    {
        public static readonly LogApiFactory Default = new LogApiFactory();

        protected override void SetupDefaultImplementations(IUnityContainer container)
        {
            container.RegisterType<IMessageHandler, NatsMessageHandler>(ReuseWithinContainer);

            container.RegisterType<ILoggerWriter, ApiLogWriter>(nameof(ApiLogEvent), ReuseWithinResolve);

            container.RegisterType<ILoggerWriter, LogRecordWriter>(nameof(LogRecordEvent), ReuseWithinResolve);


            container.RegisterFactory<Func<string, ILoggerWriter>>(c =>
                new Func<string, ILoggerWriter>(type => c.Resolve<ILoggerWriter>(type)));

            //container.RegisterType<ILoggerFactory, MyLoggerFactory>();
            container.RegisterType(typeof(ILogger<>), typeof(Logger<>));


            //var logFactory = new Microsoft.Extensions.Logging.LoggerFactory();
            //container.AddExtension(new Microsoft.Extensions.Logging.LoggingExtension(logFactory));

        }

        protected override string LoggingPrefix => "logger";
    }
}
