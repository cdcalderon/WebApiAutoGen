using Config.Enums;

namespace YPrime.BusinessLayer.Extensions
{
    public static class WebBackupTypeExtensions
    {
        public static bool IsTabletType(this WebBackupType type)
        {
            switch (type)
            {
                case WebBackupType.TabletCaregiver:
                case WebBackupType.TabletClinician:
                case WebBackupType.TabletPatient:
                    return true;
                default:
                    return false;
            }
        }
    }
}