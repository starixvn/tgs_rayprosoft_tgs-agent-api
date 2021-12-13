using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.Core.Models;
using TGS.Core.Models.GameProviders;
using TGS.Services.Operations;

namespace TGS.Apis.TgsApi.GameProviders.Jili.Services
{
    public class JiliService : IJiliService
    {
        private readonly ILogger<JiliService> _logger;
        private readonly ISettingOperations _settingOperations;

        public JiliService(
            ILogger<JiliService> logger,
            ISettingOperations settingOperations)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
        }

        public async ValueTask<JiliConfiguration> GetConfigurationAsync()
            => await _settingOperations
                .GetValueFromJsonByKeyAsync<JiliConfiguration>(SettingConsts.GameProviderConfigurations.Jili)
                .ConfigureAwait(false);
    }
}
