using System;
using System.Threading.Tasks;
using TGS.Core.Models;
using TGS.Infrastructures.Extensions;
using TGS.Services.Operations.CodeMappers;

namespace TGS.Services.Operations
{
    public interface ILicenseeGameProviderOperations
    {
        Task<string> MapCultureAsync(Guid gameProviderId, Guid licenseeId, string cultureCode);
    }

    public sealed class LicenseeGameProviderOperations : ILicenseeGameProviderOperations
    {
        private readonly ILicenseeOperations _licenseeOperations;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IGameProviderLanguageCodeMapper _languageCodeMapper;

        public LicenseeGameProviderOperations(
            ILicenseeOperations licenseeOperations,
            IGameProviderOperations gameProviderOperations,
            IGameProviderLanguageCodeMapper languageCodeMapper)
        {
            _licenseeOperations = licenseeOperations;
            _gameProviderOperations = gameProviderOperations;
            _languageCodeMapper = languageCodeMapper;
        }

        async Task<string> ILicenseeGameProviderOperations.MapCultureAsync(Guid gameProviderId, Guid LicenseeId, string cultureCode)
        {
            var internalCode = cultureCode;

            if (internalCode.IsNotNullOrEmpty())
            {
                var supported = await _gameProviderOperations.LanguageSupportedAsync(gameProviderId, cultureCode);
                if (!supported)
                {
                    var bgpc = await _licenseeOperations.GetLicenseeGameProviderConfigurationAsync(LicenseeId, gameProviderId);
                    internalCode = bgpc?.DefaultLanguageCode;
                }
            }

            if (internalCode.IsNullOrEmpty())
            {
                internalCode = Consts.DefualtCultureCode;
            }

            return await _languageCodeMapper.InternalToExternalAsync(internalCode, gameProviderId);
        }
    }
}
