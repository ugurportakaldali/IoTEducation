using Microsoft.AspNetCore.Mvc;
using IoTEducation.DataLayer;
using System.Threading.Tasks;

namespace IoTEducation.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DeviceDataController : Controller
    {
        DalDeviceData dalDeviceData;
        public DeviceDataController(DalDeviceData _dalDeviceData)
        {
            dalDeviceData = _dalDeviceData;
        }

        [HttpGet]
        [Route("Device/{Id}/last")]
        public async Task<IActionResult> GetLastByDeviceAsync(int Id)
        {
            var entity = await dalDeviceData.GetLastByDeviceAsync(Id);
            if (entity is null)
            {
                return NotFound();
            }

            return Ok(entity.ToDto());
        }
    }
}
