namespace TGS.Apis.BackendOffice.Controllers.Messages
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
        public string BackendOfficeDatabaseConnectionString { get; set; }
    }
}
