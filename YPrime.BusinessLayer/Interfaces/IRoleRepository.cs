using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.BusinessLayer.Interfaces
{
    public interface IRoleRepository
    {
        Task<List<StudyRoleModel>> GetRoles(List<string> RoleIds);

        Task<IEnumerable<StudyRoleDto>> GetUserRoles(Guid userId);
        Task<IOrderedEnumerable<StudyRoleDto>> GetAllRoles();

        Task<StudyRoleModel> GetRole(Guid Id);

        Task<StudyRoleModel> GetRole(string Id);

        List<SystemActionModel> GetRoleActions(Guid Id, bool roleIsBlinded);

        void AddActionToRole(Guid roleId, Guid systemActionId);

        void RemoveActionFromRole(Guid roleId, Guid systemActionId);

        Task<bool> UserHasRoleAction(Guid userId, string actionName);

        Task<List<ConfirmationTypeDto>> GetRoleSubscriptions(Guid Id, bool roleIsBlinded);

        void AddSubscriptionToRole(Guid roleId, Guid emailContentId);

        void RemoveSubscriptionFromRole(Guid roleId, Guid emailContentId);

        List<ReportDto> GetAllReportsByRole(Guid roleId);

        void AddReportToRole(Guid roleId, Guid reportId);

        void RemoveReportFromRole(Guid roleId, Guid reportId);

        Task<List<AnalyticsReference>> GetAllAnalyticsReferencesByRole(Guid roleId);

        void AddAnalyticsToRole(Guid roleId, Guid analyticsReferenceId);

        void RemoveAnalyticsFromRole(Guid roleId, Guid analyticsReferenceId);
    }
}