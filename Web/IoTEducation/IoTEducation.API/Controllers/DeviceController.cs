using IoTEducation.DataLayer.Entity;
using IoTEducation.Common.DTOs;
using Microsoft.AspNetCore.Mvc;
using IoTEducation.DataLayer;
using System.Threading.Tasks;
using System.Linq;

namespace IoTEducation.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DeviceController : Controller
    {
        DalDevice dalDevice;
        public DeviceController(DalDevice _dalDevice)
        {
            dalDevice = _dalDevice;
        }

        [HttpGet]
        [Route("{Id}")]
        public async Task<IActionResult> GetDeviceAsync(int Id)
        {
            var entity = await dalDevice.GetAsync(Id);
            if (entity is null)
            {
                return NotFound();
            }

            var dto = new DeviceDTO();
            dto.Id = entity.Id;
            dto.Code = entity.Code;
            dto.Name = entity.Name;
            dto.LastAccessDate = entity.LastAccessDate;

            return Ok(dto);
        }

        [HttpDelete]
        [Route("{Id}")]
        public async Task<IActionResult> DeleteDeviceAsync(int Id)
        {
            var entity = await dalDevice.GetAsync(Id);
            if (entity is null)
            {
                return NotFound();
            }

            var result = await dalDevice.DeleteAsync(entity);
            if (result == 0)
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> AddDeviceAsync(DeviceDTO dto)
        {
            var entity = new IotDevice();
            entity.Code = dto.Code;
            entity.Name = dto.Name;

            var result = await dalDevice.AddAsync(entity);
            if (result == 0)
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateDeviceAsync(DeviceDTO dto)
        {
            var entity = await dalDevice.GetAsync(dto.Id);
            if (entity is null)
            {
                return NotFound();
            }

            entity.Code = dto.Code;
            entity.Name = dto.Name;

            var result = await dalDevice.UpdateAsync(entity);
            if (result == 0)
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpGet]
        [Route("search")]
        public async Task<IActionResult> ListDeviceAsync()
        {
            var list = await dalDevice.ListAsync();
            if (list is null)
            {
                return NotFound();
            }

            var dtoList = list.Select(x => x.ToDto()).ToList();

            return Ok(dtoList);
        }
    }
}
