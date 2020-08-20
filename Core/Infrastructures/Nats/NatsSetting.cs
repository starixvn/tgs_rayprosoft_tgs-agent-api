namespace TGS.Infrastructures.Nats
{
    public class NatsSetting
    {
        public string Urls { get; set; }
        public int ConnectTimeoutMs { get; set; }
        public int PingInterval { get; set; }
        public int PingMaxOutStanding { get; set; }
        public int SubscriptionBatchSize { get; set; }
    }
}