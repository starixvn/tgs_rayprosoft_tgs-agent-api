namespace TGS.Apis.LogApi.Controllers.Messages
{
    public class StatusModel
    {
        public string MachineName { get; set; }
        public string HostName { get; set; }
        public string HostIP { get; set; }
        public string Environment { get; set; }
        public string OSVersion { get; set; }
        public int ProcessCount { get; set; }
        public bool CanConnectDB { get; set; }
    }
}
