using YPrime.Config.Enums;
using YPrime.Data.Study.Models;

namespace YPrime.BusinessLayer.Extensions
{
    public static class ReferenceMaterialExtensions
    {
        public static ReferenceMaterialType GetReferenceMaterialType(this ReferenceMaterial entity)
        {
            var enumType = ReferenceMaterialType
                .FirstOrDefault<ReferenceMaterialType>(rt => rt.Id == entity.ReferenceMaterialTypeId);
            return enumType;
        }
    }
}