namespace TGS.Clients.Report.Client.Models.GameProvider
{
    public class GameProviderRoundsModel
    {
        public string GameProviderName { get; set; }

        public string CurrencyCode { get; set; }

        public int Transactions { get; set; }

        public decimal BetAmount { get; set; }

        public decimal ValidBetAmount { get; set; }

        public decimal Turnover { get; set; }

        public decimal CompanyWinLoss { get; set; }
    }
}