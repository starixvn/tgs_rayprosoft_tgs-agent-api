namespace TGS.Apis.OperatorApi.Models
{
    public record StatusModel
    {
        public string MachineName { get; set; }
        public string HostName { get; set; }
        public string HostIp { get; set; }
        public string Environment { get; set; }
        public string OsVersion { get; set; }
        public int ProcessCount { get; set; }
        public bool CanConnectTgsDb { get; set; }
        public string TgsDatabaseConnectionString { get; set; }
    }
}
