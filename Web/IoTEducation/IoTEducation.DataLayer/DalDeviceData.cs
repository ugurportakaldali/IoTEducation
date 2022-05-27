using IoTEducation.DataLayer.Entity;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace IoTEducation.DataLayer
{
    public class DalDeviceData : DalBase<IotDeviceDatum>
    {
        public DalDeviceData(IoTEducationContext dbContext) : base(dbContext) { }

        public async Task<IotDeviceDatum> GetLastByDeviceAsync(int Id)
        {
            return await DBContext.IotDeviceData
                .Where(x => x.DeviceId == Id)
                .OrderByDescending(x => x.CreateDate)
                .Take(1).FirstOrDefaultAsync();
        }
    }
}
