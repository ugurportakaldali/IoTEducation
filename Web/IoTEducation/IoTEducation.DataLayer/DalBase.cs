using IoTEducation.DataLayer.Entity;
using System.Threading.Tasks;

namespace IoTEducation.DataLayer
{
    public class DalBase<TEntity> where TEntity : class
    {
        public readonly IoTEducationContext DBContext;
        public DalBase(IoTEducationContext dbContext)
        {
            DBContext = dbContext;
        }
      
        public async Task<int> AddAsync(TEntity rec)
        {
            await DBContext.Set<TEntity>().AddAsync(rec);
            return await DBContext.SaveChangesAsync();
        }
        public async Task<int> UpdateAsync(TEntity rec)
        {
            DBContext.Set<TEntity>().Update(rec);
            return await DBContext.SaveChangesAsync();
        }
        public async Task<int> DeleteAsync(TEntity rec)
        {
            DBContext.Set<TEntity>().Remove(rec);
            return await DBContext.SaveChangesAsync();
        }
    }
}
