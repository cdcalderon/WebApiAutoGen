using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Routing;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.BusinessLayer.Interfaces
{
    public interface IUserRepository : IBaseRepository
    {
        Task<StudyUserDto> GetUser(Guid userId, string firstName, string lastName);
        Task<IEnumerable<StudyUserDto>> GetUser(IEnumerable<Guid> userIds, string CultureCode);
        Task<string[]> GetRolesForUser(Guid userId, string siteId);
        Task<bool> IsUserInRole(string username, string roleName, string siteId);
        Task<string[]> GetUsersInRole(string roleName, string siteId);
        Task<List<StudyUserDto>> GetSiteUserByRole(Guid siteId, string roleName);
        List<StudyUserRoleDto> GetStudyUserRoles(Guid studyUserId);
        void UpsertStudyUser(StudyUserDto studyUserDto);
        void InsertStudyUserRoles(StudyUserRoleDto studyUserRoleDto);
        void ClearStudyUserRoles(Guid studyUserId);
        Task<string> GetLandingPageByUser(StudyUserDto user, RequestContext requestContext);
        bool CheckDuplicateUserSite(StudyUserRoleDto studyUserRoleDto);
        void SetYpStudyUserIdFromHeader(Guid ypStudyUserId);

        List<Site> GetValidSitesForUser(Guid userId);
    }
}