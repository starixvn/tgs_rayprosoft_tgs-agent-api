using System.ComponentModel;
using System.Threading.Tasks;

namespace TGS.Core.Infrastructures.Hangfire.Services
{
    public abstract class BaseWork<T>
    {
        [DisplayName("{0}")]
        public Task<object> ExecuteWorkerAsync(string name, object context) =>
            ExecuteAsync(context).ContinueWith(a => (object) a.Result);

        protected abstract Task<T> ExecuteAsync(object context);
    }
}