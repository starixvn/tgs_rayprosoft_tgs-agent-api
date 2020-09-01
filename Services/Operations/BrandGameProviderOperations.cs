using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.HttpOverrides;
using TGS.Core.Models;
using TGS.Infrastructures.Extensions;
using TGS.Services.Operations.CodeMappers;

namespace TGS.Services.Operations
{
    public interface IBrandGameProviderOperations
    {
        Task<string> MapCultureAsync(Guid gameProviderId, Guid brandId, string cultureCode);
    }

    public sealed class BrandGameProviderOperations : IBrandGameProviderOperations
    {
        private readonly IBrandOperations _brandOperations;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IGameProviderLanguageCodeMapper _languageCodeMapper;

        public BrandGameProviderOperations(
            IBrandOperations brandOperations,
            IGameProviderOperations gameProviderOperations,
            IGameProviderLanguageCodeMapper languageCodeMapper)
        {
            _brandOperations = brandOperations;
            _gameProviderOperations = gameProviderOperations;
            _languageCodeMapper = languageCodeMapper;
        }

        async Task<string> IBrandGameProviderOperations.MapCultureAsync(Guid gameProviderId, Guid brandId, string cultureCode)
        {
            var internalCode = cultureCode;

            if (internalCode.IsNotNullOrEmpty())
            {
                var supported = await _gameProviderOperations.LanguageSupportedAsync(gameProviderId, cultureCode);
                if (!supported)
                {
                    var bgpc = await _brandOperations.GetBrandGameProviderConfigurationAsync(brandId, gameProviderId);
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
