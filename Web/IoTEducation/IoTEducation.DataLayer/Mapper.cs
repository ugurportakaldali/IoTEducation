using IoTEducation.Common.DTOs;
using IoTEducation.DataLayer.Entity;

namespace IoTEducation.DataLayer
{
    public static class Mapper
    {
        public static DeviceDataDTO ToDto(this IotDeviceDatum entity)
        {
            return new DeviceDataDTO()
            {
                Id = entity.Id,
                DeviceId = entity.DeviceId,
                CreateDate = entity.CreateDate,
                DeviceDate = entity.DeviceDate,
                Latitude = entity.Latitude,
                Longitude = entity.Longitude,
                Humidity = entity.Humidity,
                Temperature = entity.Temperature,
                DeviceHealth = entity.DeviceHealth,
                RawData = entity.RawData
            };
        }

        public static DeviceDTO ToDto(this IotDevice entity)
        {
            return new DeviceDTO()
            {
                Id = entity.Id,
                Code = entity.Code,
                Name = entity.Name,
                LastAccessDate = entity.LastAccessDate
            };
        }
    }
}
