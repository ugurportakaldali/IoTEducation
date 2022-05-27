using System;
using System.Collections.Generic;

#nullable disable

namespace IoTEducation.DataLayer.Entity
{
    public partial class IotDeviceDatum
    {
        public long Id { get; set; }
        public int DeviceId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime DeviceDate { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public decimal Humidity { get; set; }
        public decimal Temperature { get; set; }
        public byte DeviceHealth { get; set; }
        public string RawData { get; set; }

        public virtual IotDevice Device { get; set; }
    }
}
