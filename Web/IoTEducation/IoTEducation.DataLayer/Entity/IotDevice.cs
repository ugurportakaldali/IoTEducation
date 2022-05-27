using System;
using System.Collections.Generic;

#nullable disable

namespace IoTEducation.DataLayer.Entity
{
    public partial class IotDevice
    {
        public IotDevice()
        {
            IotDeviceData = new HashSet<IotDeviceDatum>();
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public DateTime? LastAccessDate { get; set; }

        public virtual ICollection<IotDeviceDatum> IotDeviceData { get; set; }
    }
}
