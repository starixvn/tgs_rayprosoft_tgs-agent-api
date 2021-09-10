using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TGS.GameProviders.Repositories;
using TGS.Shared.Game.Models.TCG.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.TCG.Services
{
    public interface ITCGGPGameHistoryService
    {
        ValueTask<TCGGPGameRecordResponse> GameRecordAsync(TCGGPGameRecordRequest source);
    }
    public class TCGGPGameHistoryService : ITCGGPGameHistoryService
    {
        private readonly IGameProviderTCGFtpClientRepository _gameProviderTCGFtpClientRepository;
        private readonly IReadOnlyDictionary<string, string> _apiMappers = new Dictionary<string, string>() {
            {"TCG_LOTTO","/api/TCGELotto"},
            {"TCG_LOTTO_VN","/api/TCGLottoVN"},
            {"TCG_SEA","/api/TCGSEA"}
        };
        public TCGGPGameHistoryService(
            IGameProviderTCGFtpClientRepository gameProviderTCGFtpClientRepository)
        {
            _gameProviderTCGFtpClientRepository = gameProviderTCGFtpClientRepository ?? throw new ArgumentNullException(nameof(gameProviderTCGFtpClientRepository));

        }

        public async ValueTask<TCGGPGameRecordResponse> GameRecordAsync(TCGGPGameRecordRequest source)
        {
            if (!_apiMappers.TryGetValue(source.SubGP, out string api))
            {
                return new()
                {
                    Success = false,
                };
            }
            var nowDay = DateTimeOffset.Parse(DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToString("yyyy-MM-dd"));
            var lastGameRecorDirectory = (string.IsNullOrEmpty(source.LastGameRecorDirectory)) ? nowDay.ToString("yyyyMMdd") : source.LastGameRecorDirectory;
            var lastGameRecorFile = source.LastGameRecorFile;
            if (source.IsDesignation)
            {

                var content = await _gameProviderTCGFtpClientRepository.GetSettledFileContentAsync(api, source.MerchantCode, lastGameRecorDirectory, lastGameRecorFile).ConfigureAwait(false);
            }
            else
            {
                while (true)
                {
                    var getFileList = await _gameProviderTCGFtpClientRepository.GetSettledFileListAsync(api, source.MerchantCode, lastGameRecorDirectory);
                    if (getFileList == null)
                    {
                        return new()
                        {
                            Success = false,
                        };
                    }
                    if (!string.IsNullOrEmpty(lastGameRecorFile))
                    {
                        var index = Array.IndexOf(getFileList, lastGameRecorFile);
                        if (index != -1)
                        {
                            getFileList = getFileList.Where((val, idx) => idx > index).ToArray();
                        }
                    }
                    foreach (var file in getFileList)
                    {
                        var content = await _gameProviderTCGFtpClientRepository.GetSettledFileContentAsync(api, source.MerchantCode, lastGameRecorDirectory, file).ConfigureAwait(false);
                        lastGameRecorFile = file;
                    }
                    var day = DateTime.Parse($"{lastGameRecorDirectory.Substring(0, 4)}-{lastGameRecorDirectory.Substring(4, 2)}-{lastGameRecorDirectory.Substring(6, 2)}");
                    if (day.AddDays(1) <= nowDay)
                    {
                        lastGameRecorDirectory = day.AddDays(1).ToString("yyyyMMdd");
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return new()
            {
                Success = true,
                LastGameRecorDirectory = lastGameRecorDirectory,
                LastGameRecorFile = lastGameRecorFile
            };
        }
    }
}
