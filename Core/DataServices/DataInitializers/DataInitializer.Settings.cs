using System.Text.Json;
using TGS.Core.DataServices.Models;
using TGS.Core.Models;

namespace TGS.Core.DataServices.DataInitializers
{
    partial class DataInitializer
    {
        private void GenerateSettings()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };

            var settings = new Setting[]
            {
                CreateSetting(SettingConsts.GameProviderConfigurations.SBOBET, _jsonSerializationProvider.SerializeToString(SBOConfiguration, options), "JSON based configuration of SBOBET.")
            };

            _repository.BulkInsert(settings);
        }

        private static Setting CreateSetting(string key, string value, string description)
        {
            return new Setting
            {
                Key = key,
                Value = value,
                Description = description,
                CreatedByUsername = AdminUsername,
                UpdatedByUsername = AdminUsername,
                CreatedOn = _now,
                UpdatedOn = _now
            };
        }
    }
}