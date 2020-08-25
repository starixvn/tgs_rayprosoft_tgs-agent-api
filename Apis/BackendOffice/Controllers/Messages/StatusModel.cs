namespace TGS.Apis.LogApi.Controllers.Messages
{
    public class StatusModel
    {
        public string MachineName { get; set; }
        public string HostName { get; set; }
        public string HostIp { get; set; }
        public string Environment { get; set; }
        public string OsVersion { get; set; }
        public int ProcessCount { get; set; }
        public bool CanConnectBackendOfficeDb { get; set; }
    }
}
