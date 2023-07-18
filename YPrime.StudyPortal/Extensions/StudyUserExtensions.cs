using System.Linq;
using YPrime.Data.Study.Constants;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.StudyPortal.Extensions
{
    public static class StudyUserExtensions
    {
        public static bool HasPermission(this StudyUserDto studyUser, string systemAction)
        {
            return studyUser.Roles.Any(r => r.SystemActions.Any(s => s.Name == systemAction));
        }

        public static bool CanViewCaregiverTab(this StudyUserDto user)
        {
            var result = user?.Roles?.Any(r => r.SystemActions.Any(s =>
                s.Name == nameof(SystemActionTypes.CanViewCareGiverDetails) ||
                s.Name == nameof(SystemActionTypes.CanCreateCaregiverInPortal)));

            return result ?? false;
        }

        public static bool CanCreateCaregiver(this StudyUserDto user)
        {
            var result = user?.Roles?.Any(r => r.SystemActions.Any(s =>
                s.Name == nameof(SystemActionTypes.CanCreateCaregiverInPortal)));

            return result ?? false;
        }
    }
}