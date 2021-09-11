using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Json;
using TGS.GameProviders.Repositories;
using TGS.Services.GameOperations.Data;
using TGS.Shared.Game.Models.TCG.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.TCG.Services
{
    public interface ITCGGPGameHistoryService
    {
        ValueTask<TCGGPGameRecordResponse> GameRecordAsync(TCGGPGameRecordRequest source);
    }
    public class TCGGPGameHistoryService : ITCGGPGameHistoryService
    {
        private readonly IReadOnlyDictionary<string, string> _apiMappers = new Dictionary<string, string>() {
            {"TCG_LOTTO","/api/TCGELotto"},
            {"TCG_LOTTO_VN","/api/TCGLottoVN"},
            {"TCG_SEA","/api/TCGSEA"}
        };
        private readonly IReadOnlyDictionary<string, string> _productTypeMappers = new Dictionary<string, string>() {
            {"TCG_LOTTO","2"},
            {"TCG_LOTTO_VN","420"},
            {"TCG_SEA","384"}
        };
        private readonly IGameProviderTCGFtpClientRepository _gameProviderTCGFtpClientRepository;
        private readonly IGameRoundsProcessor _gameRoundsProcessor;
        private readonly ITCGService _service;
        private readonly IJsonSerializationProvider _json;

        public TCGGPGameHistoryService(
            IGameProviderTCGFtpClientRepository gameProviderTCGFtpClientRepository,
            IGameRoundsProcessor gameRoundsProcessor,
            ITCGService service,
            IJsonSerializationProvider json)
        {
            _gameProviderTCGFtpClientRepository = gameProviderTCGFtpClientRepository ?? throw new ArgumentNullException(nameof(gameProviderTCGFtpClientRepository));
            _gameRoundsProcessor = gameRoundsProcessor ?? throw new ArgumentNullException(nameof(gameRoundsProcessor));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _json = json ?? throw new ArgumentNullException(nameof(json));
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
            _productTypeMappers.TryGetValue(source.SubGP, out string externalGameId);
            var gameProviderId = await _service.GetGameProviderIdAsync().ConfigureAwait(false);
            var nowDay = DateTimeOffset.Parse(DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToString("yyyy-MM-dd"));
            var lastGameRecorDirectory = (string.IsNullOrEmpty(source.LastGameRecorDirectory)) ? nowDay.ToString("yyyyMMdd") : source.LastGameRecorDirectory;
            var lastGameRecorFile = source.LastGameRecorFile;
            if (source.IsDesignation)
            {
                string[] getFileList;
                if (string.IsNullOrEmpty(lastGameRecorFile))
                {
                    getFileList = await _gameProviderTCGFtpClientRepository.GetSettledFileListAsync(api, source.MerchantCode, lastGameRecorDirectory);
                    if (getFileList == null)
                    {
                        return new()
                        {
                            Success = false,
                        };
                    }
                }
                else
                {
                    getFileList = new[] { lastGameRecorFile };
                }

                lastGameRecorFile = await ProcessFileListAsync(gameProviderId, externalGameId, api, source.MerchantCode, lastGameRecorDirectory, getFileList).ConfigureAwait(false);
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
                    else
                    {
                        getFileList = new[] { getFileList.Last() };
                    }

                    lastGameRecorFile = await ProcessFileListAsync(gameProviderId, externalGameId, api, source.MerchantCode, lastGameRecorDirectory, getFileList).ConfigureAwait(false);
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

        private async Task<string> ProcessFileListAsync(Guid gameProviderId, string externalGameId, string api, string merchantCode, string lastGameRecorDirectory, string[] fileList)
        {
            string lastGameRecorFile = string.Empty;


            foreach (var file in fileList)
            {
                var response = await _gameProviderTCGFtpClientRepository.GetSettledFileContentAsync(api, merchantCode, lastGameRecorDirectory, file).ConfigureAwait(false);
                if (response == null)
                {
                    return lastGameRecorFile;
                }
                var tasks = new List<Task>();
                foreach (var data in response.Datas)
                {
                    var task = Task.Run(async () =>
                    {
                        var (roundsData, roundsDetail) = await _gameRoundsProcessor.GetExternalRoundIdByExternalGameIdAsync(data.OrderNum, data.PlayCode, gameProviderId, externalGameId).ConfigureAwait(false);
                        roundsDetail.AdditionalData = _json.SerializeToString(data);
                        await _gameRoundsProcessor.UpdateRoundDetailAsync(roundsDetail).ConfigureAwait(false);
                    });
                    tasks.Add(task);
                }
                await Task.WhenAll(tasks);
                lastGameRecorFile = file;
            }
            return lastGameRecorFile;
        }
    }
}
