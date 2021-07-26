using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.DataServices.Tgs;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Services.BackOffice.Domain;

namespace TGS.BackOffice.Repositories
{
    internal class BackOfficeRepository : IBackOfficeRepository
    {
        private readonly ITgsRepository _tgsRepository;

        public BackOfficeRepository(ITgsRepository tgsRepository)
        {
            _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
        }

        public async ValueTask ImportLocalizationStringsAsync(string systemCode, IEnumerable<LocalizationStringInfo> localizationStrings)
        {
            var allLocalizationStrings = _tgsRepository.SystemLocalizations.AsQueryable()
                .Where(loc => loc.SystemCode == systemCode)
                .ToArray();

            var grouping = (from loc in allLocalizationStrings
                            group loc by loc.CultureCode into g
                            select new
                            {
                                CaltureCode = g.Key,
                                Items = g.ToDictionary(loc => loc.MessageId)
                            }).ToDictionary(g => g.CaltureCode, g => g.Items);

            foreach (var info in localizationStrings)
            {
                if (grouping.TryGetValue(info.CultureCode, out var cultureDict)
                    && cultureDict.TryGetValue(info.MessageId, out var localization))
                {
                    localization.Text = info.Text;
                }
                else
                {
                    await _tgsRepository.SystemLocalizations
                        .AddAsync(new SystemLocalization
                        {
                            SystemCode = info.SystemCode,
                            CultureCode = info.CultureCode,
                            MessageId = info.MessageId,
                            Text = info.Text
                        }).ConfigureAwait(false);
                }
            }

            await _tgsRepository.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
