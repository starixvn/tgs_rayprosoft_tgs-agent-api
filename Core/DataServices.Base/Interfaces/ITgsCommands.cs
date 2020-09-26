using System.Threading.Tasks;

namespace TGS.Core.DataServices.Base.Interfaces
{
    public interface ITgsCommands<in TEntity> where TEntity : class
    {
        Task CreateAsync(TEntity obj);
        Task UpdateAsync(TEntity obj);
        Task DeleteAsync(TEntity obj);
        void BatchInsert(TEntity[] obj);
    }
}
