using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.Services.Promotion
{
    class AlwaysTrueChecker : IPromotionChecker
    {
        public ValueTask<bool> CheckAsync(string playerCode)
            => ValueTask.FromResult(true);

        public ValueTask CommitTagAsync(string playerCode) => ValueTask.CompletedTask;

        public ValueTask RollbackTagAsync(string playerCode) => ValueTask.CompletedTask;

        public ValueTask TagPromotionTypeAsync(string playerCode) => ValueTask.CompletedTask;
    }
}
