using IoTEducation.DataLayer.Entity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace IoTEducation.DataLayer
{
    public class DalDevice : DalBase<IotDevice>
    {
        public DalDevice(IoTEducationContext dbContext) : base(dbContext) { }

        public async Task<IotDevice> GetAsync(int id)
        {
            return await DBContext.IotDevices
                                        .Where(x => x.Id == id)
                                        .FirstOrDefaultAsync();
        }
        public async Task<List<IotDevice>> ListAsync()
        {
            return await DBContext.IotDevices
                                        .ToListAsync();
        }
    }
}
