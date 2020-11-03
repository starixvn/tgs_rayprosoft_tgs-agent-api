using System;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.GameProviders.Settings;

namespace TGS.Services.Operations
{
    public interface IPluggableOperations
    {
        Task<Uri> GetEndpointUrlByGameProviderCodeAsync(string gameProviderCode);
    }

    public sealed class PluggableOperations : IPluggableOperations
    {
        private readonly ISettingOperations _settingOperations;

        public PluggableOperations(ISettingOperations settingOperations)
        {
            _settingOperations = settingOperations;
        }


        async Task<Uri> IPluggableOperations.GetEndpointUrlByGameProviderCodeAsync(string gameProviderCode)
        {
            var config = await _settingOperations.GetValueFromJsonByKeyAsync<PluggableConfigurationBase>(
                SettingConsts.GameProviderConfigurations.Template.Args(gameProviderCode));

            return config.ServiceEndpointUrl;
        }
    }
}
