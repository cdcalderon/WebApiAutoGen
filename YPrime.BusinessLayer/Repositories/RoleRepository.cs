using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using YPrime.BusinessLayer.BaseClasses;
using YPrime.BusinessLayer.Enums;
using YPrime.BusinessLayer.Interfaces;
using YPrime.BusinessLayer.Session;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;
using YPrime.eCOA.DTOLibrary.Utils;

namespace YPrime.BusinessLayer.Repositories
{
    public class RoleRepository : BaseRepository, IRoleRepository
    {
        private readonly IStudyRoleService _studyRoleService;

        public RoleRepository(
            IStudyDbContext db, 
            IStudyRoleService studyRoleService) 
            : base(db)
        {
            _studyRoleService = studyRoleService;
        }

        public async Task<List<StudyRoleModel>> GetRoles(List<string> roleNames)
        {
            roleNames = roleNames ?? new List<string>();
            var allRoles = await _studyRoleService.GetAll();
            var res = allRoles.Where(u => !roleNames.Any() || roleNames.Contains(u.ShortName))
                .ToList()
                .Select(u =>
                    {
                        var result = new StudyRoleModel();
                        result.CopyPropertiesFromObject(u);
                        return result;
                    }
                ).ToList();
            return res;
        }

        public async Task<IOrderedEnumerable<StudyRoleDto>> GetAllRoles()
        {
            var allStudyRoles = await _studyRoleService.GetAll();
            var studyRoleUpdates = _db.StudyRoleUpdates.ToList();
            var result = allStudyRoles
                .Select(d => new StudyRoleDto
                {
                    Id = d.Id,
                    ShortName = d.ShortName,
                    IsBlinded = d.IsBlinded,
                    Description = d.Description,
                    LastUpdate = studyRoleUpdates.FirstOrDefault(sru => sru.StudyRoleId == d.Id)?.LastUpdate
                })
                .OrderBy(p => p.Id);

            return result;
        }

        public async Task<StudyRoleModel> GetRole(Guid Id)
        {
            var allStudyRoles = await _studyRoleService.GetAll();       
            return allStudyRoles.Single(d => d.Id == Id);
        }

        public async Task<StudyRoleModel> GetRole(string shortName)
        {           
            var allStudyRoles = await _studyRoleService.GetAll();
            return allStudyRoles.Single(d => d.ShortName == shortName);
        }

        public async Task<IEnumerable<StudyRoleDto>> GetUserRoles(Guid userId)
        {
            var allRoles = await _studyRoleService.GetAll();
            var studyUserRoles = _db.StudyUserRoles.Where(sur => sur.StudyUserId == userId);
            var studyRoleIds = studyUserRoles.Select(x => x.StudyRoleId);

            var result = allRoles.Where(x => studyRoleIds.Contains(x.Id)).Select(
                sr => new StudyRoleDto
                {
                    Id = sr.Id,
                    ShortName = sr.ShortName,
                    IsBlinded = sr.IsBlinded,
                    Description = sr.Description
                }).Distinct();

            return result;
        }

        public List<SystemActionModel> GetRoleActions(Guid roleId, bool roleIsBlinded)
        {
            List<SystemActionModel> result;       

            var systemActionsStudyRoles = _db.SystemActionStudyRoles.Where(x => x.StudyRoleId == roleId).ToList();

            if (roleIsBlinded)
            {
                result = systemActionsStudyRoles.Where(sasr => sasr.SystemAction.IsBlinded).Select(s =>
                {
                    var systemActionModel = new SystemActionModel();
                    systemActionModel.CopyPropertiesFromObject(s.SystemAction);
                    return systemActionModel;
                }).ToList();
            }
            else
            {
                result = systemActionsStudyRoles.Select(s =>
                {
                    var systemActionModel = new SystemActionModel();
                    systemActionModel.CopyPropertiesFromObject(s.SystemAction);
                    return systemActionModel;
                }).ToList();
            }

            return result;
        }

        public async Task<bool> UserHasRoleAction(Guid userId, string actionName)
        {
            var roles = await GetUserRoles(userId);

            foreach (var role in roles)
            {
                var actions = GetRoleActions(role.Id, role.IsBlinded);

                if (actions.Any(a => a.Name == actionName))
                {
                    return true;
                }
            }

            return false;
        }
        
        public void AddActionToRole(Guid roleId, Guid systemActionId)
        {
            var entity = new SystemActionStudyRole
            {
                StudyRoleId = roleId,
                SystemActionId = systemActionId
            };
            
            _db.SystemActionStudyRoles.Add(entity);
            _db.SaveChanges(YPrimeSession.Instance?.CurrentUser?.Id.ToString());

            var systemActionEntity = _db.SystemActions.FirstOrDefault(s => s.Id == systemActionId);

            if (systemActionEntity != null)
            {
                var systemActionModel = new SystemActionModel();
                systemActionModel.CopyPropertiesFromObject(systemActionEntity);

                YPrimeSession.Instance.CurrentUser.Roles.SingleOrDefault(r => r.Id == roleId)?
                .SystemActions.Add(systemActionModel);
            }

            SetRoleLastUpdated(roleId);
        }

        public void RemoveActionFromRole(Guid roleId, Guid systemActionId)
        {
            var entity = _db.SystemActionStudyRoles.Single(sasr =>
                sasr.StudyRoleId == roleId && sasr.SystemActionId == systemActionId);
            _db.SystemActionStudyRoles.Remove(entity);
            _db.SaveChanges(YPrimeSession.Instance?.CurrentUser?.Id.ToString());

            var systemAction = YPrimeSession.Instance.CurrentUser.Roles.SingleOrDefault(r => r.Id == roleId)?
                .SystemActions.SingleOrDefault(s => s.Id == systemActionId);

            if (systemAction != null)
            {
                YPrimeSession.Instance.CurrentUser.Roles.SingleOrDefault(r => r.Id == roleId)
                .SystemActions.Remove(systemAction);
            }

            SetRoleLastUpdated(roleId);
        }

        public async Task<List<ConfirmationTypeDto>> GetRoleSubscriptions(Guid Id, bool roleIsBlinded)
        {
            List<ConfirmationTypeDto> result;
            var studyRoles = await _studyRoleService.GetAll();
            var role = studyRoles.SingleOrDefault(r => r.Id == Id);
            var emailContents = _db.EmailContentStudyRoles.Where(rbs => rbs.StudyRoleId == role.Id).Select(x => x.EmailContent).ToList();
            if (roleIsBlinded)
            {
                result = emailContents.Where(ec => ec.IsBlinded).Select(e =>
                {
                    var confirmationTypeDto = new ConfirmationTypeDto();
                    confirmationTypeDto.CopyPropertiesFromObject(e, false);
                    return confirmationTypeDto;
                }).ToList();
            }
            else
            {
                result = emailContents.Select(e =>
                {
                    var confirmationTypeDto = new ConfirmationTypeDto();
                    confirmationTypeDto.CopyPropertiesFromObject(e);
                    return confirmationTypeDto;
                }).ToList();
            }

            return result;
        }

        public void AddSubscriptionToRole(Guid roleId, Guid emailContentId)
        {
            var entity = new EmailContentStudyRole()
            {
                StudyRoleId = roleId,
                EmailContentId = emailContentId
            };
            _db.EmailContentStudyRoles.Add(entity);
            _db.SaveChanges(YPrimeSession.Instance?.CurrentUser?.Id.ToString());

            SetRoleLastUpdated(roleId);
        }

        public void RemoveSubscriptionFromRole(Guid roleId, Guid emailContentId)
        {
            var entity = _db.EmailContentStudyRoles.Single(rbs => rbs.StudyRoleId == roleId && rbs.EmailContentId == emailContentId);
            _db.EmailContentStudyRoles.Remove(entity);
            _db.SaveChanges(YPrimeSession.Instance?.CurrentUser?.Id.ToString());

            SetRoleLastUpdated(roleId);
        }

        public List<ReportDto> GetAllReportsByRole(Guid roleId)
        {
            return _db.ReportStudyRoles.Where(x => x.StudyRoleId == roleId)
                .Select(x => new ReportDto
                {
                    Id = x.ReportId,
                    ReportName = x.ReportName
                }).ToList();
        }

        public void AddReportToRole(Guid roleId, Guid reportId)
        {
            var report = ReportType.FirstOrDefault<ReportType>(r => r.Id == reportId);

            _db.ReportStudyRoles.Add(new ReportStudyRole
            {
                ReportId = reportId,
                StudyRoleId = roleId,
                ReportName = report.Name
            });

            _db.SaveChanges(YPrimeSession.Instance?.CurrentUser?.Id.ToString());

            SetRoleLastUpdated(roleId);
        }

        public void RemoveReportFromRole(Guid roleId, Guid reportId)
        {
            var reportStudyRole = new ReportStudyRole
            {
                ReportId = reportId,
                StudyRoleId = roleId
            };

            _db.ReportStudyRoles.Attach(reportStudyRole);
            _db.ReportStudyRoles.Remove(reportStudyRole);
            _db.SaveChanges(YPrimeSession.Instance?.CurrentUser?.Id.ToString());

            SetRoleLastUpdated(roleId);
        }

        public async Task<List<AnalyticsReference>> GetAllAnalyticsReferencesByRole(Guid roleId)
        {
            return await _db.AnalyticsReferenceStudyRoles
                .Where(ar => ar.StudyRoleId == roleId)
                .Select(ar => ar.AnalyticsReference)
                .ToListAsync();
        }

        public void AddAnalyticsToRole(Guid roleId, Guid analyticsReferenceId)
        {
            _db.AnalyticsReferenceStudyRoles.Add(new AnalyticsReferenceStudyRole
            {
                AnalyticsReferenceId = analyticsReferenceId,
                StudyRoleId = roleId
            });

            _db.SaveChanges(YPrimeSession.Instance?.CurrentUser?.Id.ToString());

           SetRoleLastUpdated(roleId);
        }

        public void RemoveAnalyticsFromRole(Guid roleId, Guid analyticsReferenceId)
        {
            var analyticsReferenceStudyRole = _db.AnalyticsReferenceStudyRoles.Single(ar => ar.StudyRoleId == roleId && ar.AnalyticsReferenceId == analyticsReferenceId);
            _db.AnalyticsReferenceStudyRoles.Remove(analyticsReferenceStudyRole);
            _db.SaveChanges(YPrimeSession.Instance?.CurrentUser?.Id.ToString());

            SetRoleLastUpdated(roleId);
        }

        private void SetRoleLastUpdated(Guid roleId)
        {
            var studyRoleUpdate = new StudyRoleUpdate
            {
                StudyRoleId = roleId,
                LastUpdate = DateTime.Now
            };

            _db.StudyRoleUpdates.AddOrUpdate(sru => sru.StudyRoleId, studyRoleUpdate);
            _db.SaveChanges(YPrimeSession.Instance?.CurrentUser?.Id.ToString());
        }
    }
}