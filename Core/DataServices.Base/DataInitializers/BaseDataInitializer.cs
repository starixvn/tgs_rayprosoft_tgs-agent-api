using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using TGS.Core.DataServices.Base.Interfaces;

namespace TGS.Core.DataServices.Base.DataInitializers
{
    public abstract class BaseDataInitializer : IDataInitializer
    {
        private static readonly object Locker = new();

        Task IDataInitializer.SetInitialDataAsync() => SetInitialDataAsync();

        protected abstract Task SetInitialDataAsync();

        protected static async Task LogSeedingTimeAsync(Func<Task> getFunc,
            StringBuilder builder)
        {
            var timer = Stopwatch.StartNew();
            await getFunc();
            timer.Stop();
            lock (Locker)
            {
                builder.AppendLine($"{getFunc.Method.Name} total spent {timer.ElapsedMilliseconds} ms");
            }
        }
    }
}
