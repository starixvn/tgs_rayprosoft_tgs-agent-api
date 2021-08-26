using System;
using System.Globalization;

namespace TGS.Apis.TgsApi.GameProviders.WMCasino.Services
{
    public interface IWMCasinoUtility
    {
        public string GetExternalRoundIdByGameNumber(string gameNumber, string externalGameId);

        public string GetExternalRoundIdByBetResult(string roundId, string subRoundId);

        public string GetTimestamp();

        public DateTimeOffset ParseDateTimeOffset(string timestamp);
    }

    public class WMCasinoUtility : IWMCasinoUtility
    {
        private const string _TimestampFormat = "yyyy-MM-dd HH:mm:ss";
        private static readonly TimeSpan _Offset = TimeSpan.FromHours(8);

        public string GetExternalRoundIdByGameNumber(string gameNumber, string externalGameId)
            => gameNumber.Replace($"{externalGameId}_", string.Empty);

        public string GetExternalRoundIdByBetResult(string roundId, string subRoundId)
            => $"{roundId}_{subRoundId}";

        public string GetTimestamp()
            => DateTimeOffset.UtcNow.ToOffset(_Offset).ToString(_TimestampFormat);

        public DateTimeOffset ParseDateTimeOffset(string timestamp)
        {
            var dateTime = DateTime.ParseExact(timestamp, _TimestampFormat, null,
                DateTimeStyles.AllowWhiteSpaces);

            return new DateTimeOffset(dateTime, _Offset);
        }
    }
}