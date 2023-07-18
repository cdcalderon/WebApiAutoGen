using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using YPrime.BusinessLayer.BaseClasses;
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
    public class UserRepository : BaseRepository, IUserRepository
    {
        private readonly IAuthenticationUserRepository _authenticationUserRepository;
        private readonly IStudyRoleService _studyRoleService;
        private readonly ISoftwareReleaseRepository _softwareReleaseRepository;

        public UserRepository(IStudyDbContext db,
            IAuthenticationUserRepository authenticationUserRepository,
            IStudyRoleService studyRoleService,
            ISoftwareReleaseRepository softwareReleaseRepository) : base(db)
        {
            _authenticationUserRepository = authenticationUserRepository;
            _studyRoleService = studyRoleService;
            _softwareReleaseRepository = softwareReleaseRepository;
        }

        public void SetYpStudyUserIdFromHeader(Guid ypStudyUserId)
        {
            if (_db == null)
            {
                return;
            }
            _db.YpStudyUserIdFromHeader = ypStudyUserId;
        }

        public async Task<string[]> GetRolesForUser(Guid userId, string siteId)
        {
            var studyRoles = await _studyRoleService.GetAll();
            var studyUserRoles = _db.StudyUserRoles.Where(r => r.StudyUserId == userId
                                                 && (siteId == null || r.Site.SiteNumber == siteId));

            var studyRoleIds = studyUserRoles.Select(x => x.StudyRoleId).ToList();
            var results = studyRoles.Where(x => studyRoleIds.Contains(x.Id));

            return results.Select(x => x.ShortName).ToArray();
        }


        public async Task<StudyUserDto> GetUser(Guid userId, string firstName, string lastName)

        {
            var user = _db.StudyUsers.Single(x => x.Id == userId);

            var result = await Adapt(user);
            result.FirstName = firstName;
            result.LastName = lastName;
            result.Email = user.Email;

            return result;
        }


        public List<Site> GetValidSitesForUser(Guid userId)
        {
            var userSites = _db.StudyUserRoles
            .Where(r => r.StudyUserId == userId)
            .Include(r => r.Site)
            .ToList()
            .Select(s =>
            {
                return s.Site;
            })
            .GroupBy(s => s.Id)
            .Select(sg => sg.First())
            .ToList();

            return userSites;
        }

        public async Task<IEnumerable<StudyUserDto>> GetUser(IEnumerable<Guid> userIds, string CultureCode)
        {
            IEnumerable<dynamic> authenticationUser = await _authenticationUserRepository.GetUsersAsync(userIds);

            var users = _db.StudyUsers.Where(x => userIds.Contains(x.Id))
                .ToDictionary(x => x.Id, x => x);

            List<StudyUserDto> studyUserDtos = new List<StudyUserDto>();

            foreach (var user in authenticationUser)
            {
                var result = await Adapt(users[Guid.Parse(user.StudyUser.Id.Value)]);
                result.FirstName = user.FirstName.Value;
                result.LastName = user.LastName.Value;
                result.PhoneNumber = user.StudyUser.PhoneNumber.Value;
                result.Email = user.StudyUser.Email.Value;
                studyUserDtos.Add(result);
            }

            return studyUserDtos;
        }

        public async Task<bool> IsUserInRole(string username, string roleName, string siteId)
        {
            var studyRoles = await _studyRoleService.GetAll();

            var userRoleIds = _db.StudyUserRoles
                .Where(r =>
                    r.StudyUserId == Guid.Parse(username) &&
                    (siteId == null || r.SiteId == Guid.Parse(siteId)))
                .Select(ur => ur.StudyRoleId);

            return studyRoles.Where(sr => userRoleIds.Contains(sr.Id)).Any(r => r.ShortName == roleName);
        }

        public async Task<string[]> GetUsersInRole(string roleName, string siteId)
        {
            var studyRoles = await _studyRoleService.GetAll();
            var role = studyRoles.FirstOrDefault(s => s.ShortName == roleName);

            return _db.StudyUserRoles
                .Where(r =>
                    r.StudyRoleId == role.Id &&
                    (siteId == null || r.SiteId == Guid.Parse(siteId)))
                .Select(r => r.StudyUser.UserName)
                .ToArray();
        }

        public async Task<List<StudyUserDto>> GetSiteUserByRole(Guid siteId, string roleName)
        {
            var studyRoles = await _studyRoleService.GetAll();
            var role = studyRoles.FirstOrDefault(s => s.ShortName == roleName);
            var users = _db.StudyUserRoles.Where(x => x.SiteId == siteId && x.StudyRoleId == role.Id)
                .Select(x => x.StudyUser);

            var result = new List<StudyUserDto>();

            foreach (var user in users)
            {
                var dto = new StudyUserDto();
                dto.CopyPropertiesFromObject(user);
                result.Add(dto);
            }

            return result;
        }

        public List<StudyUserRoleDto> GetStudyUserRoles(Guid studyUserId)
        {
            var studyUserRoles = _db.StudyUserRoles
                .Where(s => s.StudyUserId == studyUserId)
                .Select(s =>
                    new StudyUserRoleDto
                    {
                        Id = s.Id,
                        SiteId = s.SiteId,
                        StudyRoleId = s.StudyRoleId,
                        StudyUserId = s.StudyUserId,
                        SiteIsActive = s.Site.IsActive,
                        Notes = s.Notes
                    }).ToList();
            return studyUserRoles;
        }

        public void UpsertStudyUser(StudyUserDto studyUserDto)
        {
            var studyUser = new StudyUser
            {
                Id = studyUserDto.Id,
                UserName = studyUserDto.UserName,
                Email = studyUserDto.Email
            };
            _db.StudyUsers.AddOrUpdate(studyUser);
            _db.SaveChanges(YPrimeSession.Instance?.CurrentUser?.Id.ToString());
        }

        public void InsertStudyUserRoles(StudyUserRoleDto studyUserRoleDto)
        {
            var studyUserRole = new StudyUserRole
            {
                Id = studyUserRoleDto.Id,
                StudyUserId = studyUserRoleDto.StudyUserId,
                StudyRoleId = studyUserRoleDto.StudyRoleId,
                SiteId = studyUserRoleDto.SiteId,
                Notes = studyUserRoleDto.Notes
            };
            _db.StudyUserRoles.Add(studyUserRole);
            _db.SaveChanges(YPrimeSession.Instance?.CurrentUser?.Id.ToString());
        }

        public void ClearStudyUserRoles(Guid studyUserId)
        {
            _db.StudyUserRoles.RemoveRange(_db.StudyUserRoles.Where(u => u.StudyUserId == studyUserId));
            _db.SaveChanges(YPrimeSession.Instance?.CurrentUser?.Id.ToString());
        }

        public async Task<string> GetLandingPageByUser(StudyUserDto user, RequestContext requestContext)
        {
            string result = null;
            var defaultActionName = "Index";
            var defaultControllerName = "Dashboard";
            var studyUser = _db.StudyUsers.Single(su => su.Id == user.Id);

            if (studyUser.LandingPageUrl != null && !string.IsNullOrWhiteSpace(studyUser.LandingPageUrl))
            {
                result = studyUser.LandingPageUrl;
            }
            else
            {
                var studyRoles = await _studyRoleService.GetAll();
                var userRoles = GetStudyUserRoles(user.Id).Select(sur => sur.StudyRoleId).ToList();

                var landingPages = studyRoles.Where(sr =>
                    userRoles.Contains(sr.Id) && sr.LandingPageName != null &&
                    sr.LandingPageName.Trim() != "").ToList();
                UrlHelper u = new UrlHelper(requestContext);
                if (landingPages.Any())
                {
                    switch (landingPages.First().LandingPageId)
                    {
                        case 1:
                            result = u.Action(defaultActionName,
                        "Dashboard", null);
                            break;
                        case 2:
                            result = u.Action(defaultActionName,
                     "Patient", null);
                            break;
                        case 3:
                            result = u.Action(defaultActionName,
                     "Site", null);
                            break;
                        case 4:
                            result = u.Action(defaultActionName,
                     "Analytics", null);
                            break;
                        case 5:
                            result = u.Action(defaultActionName,
                     "ReferenceMaterial", null);
                            break;
                    }

                }
                else
                {
                    result = u.Action(defaultActionName, defaultControllerName, null);
                }
            }

            return result;
        }

        public bool CheckDuplicateUserSite(StudyUserRoleDto studyUserRoleDto)
        {
            return _db.StudyUserRoles.Any(x =>
                x.StudyUserId == studyUserRoleDto.StudyUserId && x.SiteId == studyUserRoleDto.SiteId &&
                x.StudyRoleId == studyUserRoleDto.StudyRoleId);
        }

        private async Task<StudyUserDto> Adapt(StudyUser user)
        {
            var result = new StudyUserDto();
            result.CopyPropertiesFromObject(user);

            var userSites = GetValidSitesForUser(user.Id)
                .Select(s =>
                {
                    var site = new SiteDto();
                    site.CopyPropertiesFromObject(s);
                    return site;
                })
                .ToList();

            var configId = await _softwareReleaseRepository.FindLatestConfigurationVersion(
                userSites.Select(s => s.Id).ToList(),
                userSites.Select(s => s.CountryId).ToList());

            var allRoles = await _studyRoleService.GetAll(configId);
            var userRoleIds = user.StudyUserRoles.Select(u => u.StudyRoleId);

            List<StudyRoleModel> studyRoles = new List<StudyRoleModel>();
            var systemActionsStudyRoles = _db.SystemActionStudyRoles.Where(x => userRoleIds.Contains(x.StudyRoleId)).ToList();

            foreach (var s in user.StudyUserRoles.GroupBy(sur => sur.StudyRoleId))
            {
                var studyRole = allRoles.FirstOrDefault(x => x.Id == s.Key);
                if (studyRole != null)
                {
                    var systemActions = systemActionsStudyRoles
                            .Where(x => x.StudyRoleId == studyRole.Id && (!studyRole.IsBlinded || x.SystemAction.IsBlinded))
                            .Select(sr => new SystemActionModel
                            {
                                ActionLocation = sr.SystemAction.ActionLocation,
                                Description = sr.SystemAction.Description,
                                Id = sr.SystemAction.Id,
                                IsBlinded = sr.SystemAction.IsBlinded,
                                Name = sr.SystemAction.Name
                            }).ToList();

                    studyRole.SystemActions = systemActions;

                    studyRoles.Add(studyRole);
                }
            }

            result.Roles = studyRoles;
            result.Sites = userSites;

            return result;
        }
    }
}