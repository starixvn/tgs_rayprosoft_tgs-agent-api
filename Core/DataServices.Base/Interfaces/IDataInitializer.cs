using System.Threading.Tasks;

namespace TGS.Core.DataServices.Base.Interfaces
{
    public interface IDataInitializer
    {
        Task SetInitialDataAsync();
    }
}
