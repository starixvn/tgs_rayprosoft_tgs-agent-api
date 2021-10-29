namespace TGS.Services.BackOffice.Application.Models.Withdraw
{
    public class ExportWithdrawModel
    {
        public int Sequence { get; init; }
        public string Username { get; init; }
        public string UseType { get; init; }
        public string Id { get; init; }
        public string ThirdPartyOrderNo { get; init; }
        public decimal Amount { get; init; }
        public string ApplicationTime { get; init; }
        public string UpdateTime { get; init; }
        public string Status { get; init; }
        public string Remark { get; init; }
    }
}