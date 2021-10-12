using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace TGS.Apis.TgsApi.GameProviders.WMCasino.Services
{
    public interface IWMCasinoUtility
    {
        public string GetExternalRoundIdByGameNumber(string gameNumber, string externalGameId);

        public string GetExternalRoundIdByBetResult(string roundId, string subRoundId);

        public string GetTimestamp();

        public DateTimeOffset ParseDateTimeOffset(string timestamp);

        public decimal GetDepositAmount(string betType, decimal betAmount);
    }

    public class WMCasinoUtility : IWMCasinoUtility
    {
        private const string _TimestampFormat = "yyyy-MM-dd HH:mm:ss";
        private static readonly TimeSpan _Offset = TimeSpan.FromHours(8);

        private static readonly IReadOnlyDictionary<string, decimal> _DepositRates =
            new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase)
            {
                ["Player1Double"] = 5m,
                ["Player2Double"] = 5m,
                ["Player3Double"] = 5m
            };

        public string GetExternalRoundIdByGameNumber(string gameNumber, string externalGameId)
        {
            var values = gameNumber.Split('_', StringSplitOptions.RemoveEmptyEntries)
                .Where(v => v != externalGameId);

            return string.Join("_", values);
        }

        public string GetExternalRoundIdByBetResult(string roundId, string subRoundId)
            => $"{roundId}_{subRoundId}";

        public string GetTimestamp()
            => DateTimeOffset.UtcNow.ToOffset(_Offset).ToString(_TimestampFormat);

        public DateTimeOffset ParseDateTimeOffset(string timestamp)
        {
            var dateTime = DateTime.ParseExact(timestamp,
                _TimestampFormat,
                null,
                DateTimeStyles.AllowWhiteSpaces);

            return new DateTimeOffset(dateTime, _Offset);
        }

        public decimal GetDepositAmount(string betType, decimal betAmount)
        {
            if (!_DepositRates.TryGetValue(betType, out var rate))
            {
                rate = 1;
            }

            return betAmount * rate;
        }
    }
}