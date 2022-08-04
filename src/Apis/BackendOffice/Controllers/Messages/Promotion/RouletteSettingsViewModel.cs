using System;
using System.Collections.Generic;

namespace TGS.Apis.BackendOffice.Controllers.Messages.Promotion
{
    public class RouletteSettingsViewModel
    {
        public IEnumerable<RouletteItemViewModel> Items { get; set; } = Array.Empty<RouletteItemViewModel>();
    }
}
