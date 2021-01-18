using TGS.Core.Infrastructures.Hangfire;

namespace TGS.Worker.Schedulers
{
    public class AppConfig
    {
        public HangfireSettings Hangfire { get; set; }
    }
}