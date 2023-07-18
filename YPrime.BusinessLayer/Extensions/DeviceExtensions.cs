using YPrime.Config.Enums;
using YPrime.Data.Study.Models;

namespace YPrime.BusinessLayer.Extensions
{
    public static class DeviceExtensions
    {
        public static DeviceType GetDeviceType(this Device entity)
        {
            var enumType = DeviceType
                .FirstOrDefault<DeviceType>(et => et.Id == entity.DeviceTypeId);
            return enumType;
        }
    }
}