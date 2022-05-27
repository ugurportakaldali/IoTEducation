using System;

namespace IoTEducation.Common.DTOs
{
    public class DeviceDTO
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public DateTime? LastAccessDate { get; set; }
    }
}
