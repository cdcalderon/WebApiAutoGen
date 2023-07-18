using YPrime.Config.Enums;
using YPrime.Data.Study.Models;

namespace YPrime.BusinessLayer.Extensions
{
    public static class WidgetExtensions
    {
        public static WidgetType GetWidgetType(this Widget entity)
        {
            var enumType = WidgetType
                .FirstOrDefault<WidgetType>(et => et.Id == entity.WidgetTypeId);

            return enumType;
        }
    }
}