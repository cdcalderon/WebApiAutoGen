using YPrime.Core.BusinessLayer.Interfaces;

namespace YPrime.Core.BusinessLayer.Extensions
{
    public static class ServiceSettingsExtensions
    {
        public static bool IsProductionEnvironment(this IServiceSettings settings)
        {
            return settings?.StudyPortalAppEnvironment?.ToUpper() == Config.Constants.Environment.Prod;
        }
    }
}
