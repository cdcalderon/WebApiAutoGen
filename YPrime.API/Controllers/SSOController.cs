using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
using YPrime.Auth.Data.Models.JSON;
using YPrime.BusinessLayer.Interfaces;
using YPrime.BusinessLayer.Session;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.API.Controllers
{
    public class SSOController : BaseApiController
    {
        private readonly ISiteRepository _siteRepository;
        private readonly IStudyRoleService _studyRoleService;
        private readonly IUserRepository _userRepository;

        private const string _ypStudyUserIdHeaderName = "YP-Study-User-ID";

        private void SetYpStudyUserIdFromHeader()
        {
            var ypStudyUserIdHeader = System.Web.HttpContext.Current.Request.Headers[_ypStudyUserIdHeaderName];
            if (ypStudyUserIdHeader != null)
            {
                Guid tempYpStudyUserId; // Inline variable declaration causes build error.
                if (Guid.TryParse(ypStudyUserIdHeader, out tempYpStudyUserId))
                {
                    _userRepository.SetYpStudyUserIdFromHeader(tempYpStudyUserId);
                }
            }
        }

        public SSOController(ISiteRepository siteRepository,
            IStudyRoleService studyRoleService,
            IUserRepository studyUserRepository,
            ISessionService sessionService) : base(sessionService)
        {
            _siteRepository = siteRepository;
            _userRepository = studyUserRepository;
            _studyRoleService = studyRoleService;

            SetYpStudyUserIdFromHeader();
        }

        // GET: GetStudyData
        [HttpGet]
        public async Task<dynamic> GetStudyData(Guid? userId)
        {
            var studyRoles = await GetStudyRoles();
            List<StudyRole> roles = new List<StudyRole>();

            //Roles & sites
            foreach (var role in studyRoles)
            {
                var studyRole = new StudyRole()
                {
                    Description = role.Description,
                    Id = role.Id,
                    IsBlinded = role.IsBlinded,
                    LastUpdate = role.LastUpdate,
                    Notes = role.Notes,
                    ShortName = role.ShortName
                };

                roles.Add(studyRole);
            }

            var studyData = new StudyData
            {
                StudyRoles = roles,
                StudySites = await GetStudySites()
            };

            //StudyUserRoles if they pass a user
            if (userId != null)
            {
                var studyUserRoles = _userRepository.GetStudyUserRoles((Guid)userId);
                foreach (var studyUserRole in studyUserRoles)
                {
                    studyData.StudyUserRoles.Add(new StudyUserRole
                    {
                        StudyUserId = studyUserRole.StudyUserId,
                        StudyRoleId = studyUserRole.StudyRoleId,
                        SiteId = studyUserRole.SiteId
                    });
                }
            }

            return Json(studyData);
        }

        // POST: PostUserData
        [HttpPost]
        [Route("api/SSO/PostUserData")]
        public IHttpActionResult PostUserData(dynamic userDataJson)
        {
            UserData userData = JsonConvert.DeserializeObject<UserData>(Convert.ToString(userDataJson));
            SaveUserData(userData);
            return Ok();
        }

        // POST: ImportUserData
        [HttpPost]
        public async Task<dynamic> ImportUserData(dynamic userDataJson)
        {
            List<UserData> userData = JsonConvert.DeserializeObject<List<UserData>>(Convert.ToString(userDataJson));
            var errors = await InflateStudyDataByName(userData);
            if (errors.Count == 0)
            {
                foreach (var user in userData)
                {
                    SaveUserData(user, true);
                }
            }

            return Json(new { Errors = errors });
        }

        [HttpGet]
        public IHttpActionResult CheckIfStudyAcceptsSiteNumber()
        {
            //return ok to call to validate import with sitenumber if this api exists
            return Ok();
        }


        private void SaveUserData(UserData userData, bool isImport = false)
        {
            //Set studyUser & upsert
            var studyUser = new StudyUserDto
            {
                Id = userData.StudyUser.Id,
                Email = userData.StudyUser.Email,
                UserName = userData.StudyUser.UserName
            };
            _userRepository.UpsertStudyUser(studyUser);

            if (!isImport)
            {
                //Clear existing studyRoles
                _userRepository.ClearStudyUserRoles(userData.StudyUser.Id);
            }

            //Insert new studyRoles
            foreach (var s in userData.StudyUserRoles)
            {
                var studyUserRole = new StudyUserRoleDto
                {
                    StudyUserId = studyUser.Id,
                    StudyRoleId = s.StudyRoleId,
                    SiteId = s.SiteId,
                    Id = (s.Id == Guid.Empty) ? Guid.NewGuid() : s.Id
                };

                //checking if same record already exists or not
                if (!_userRepository.CheckDuplicateUserSite(studyUserRole))
                {
                    _userRepository.InsertStudyUserRoles(studyUserRole);
                }
            }
        }

        private async Task<List<StudySite>> GetStudySites()
        {
            var sites = await _siteRepository.GetAllSites();

            return sites.Select(studySite => new StudySite
            {
                Id = studySite.Id,
                SiteId = studySite.SiteNumber,
                SiteName = studySite.Name,
                SiteAddress1 = studySite.Address1,
                SiteAddress2 = studySite.Address2,
                SiteAddress3 = studySite.Address3,
                SiteCity = studySite.City,
                SiteState = studySite.State,
                SiteZip = studySite.Zip,
                CountryId = studySite.CountryId,
                PrimaryContact = studySite.PrimaryContact,
                SitePhoneNumber = studySite.PhoneNumber,
                SiteFaxNumber = studySite.FaxNumber,
                SiteIsActive = studySite.IsActive
            })
            .ToList();
        }

        private async Task<List<StudyRoleModel>> GetStudyRoles()
        {
            var studyRoles = new List<StudyRoleModel>();
            var allRoles = await _studyRoleService.GetAll();
            foreach (var studyRole in allRoles)
            {
                studyRoles.Add(new StudyRoleModel
                {
                    Id = studyRole.Id,
                    ShortName = studyRole.ShortName,
                    Description = studyRole.Description,
                    IsBlinded = studyRole.IsBlinded,
                    Notes = studyRole.Notes
                });
            }

            return studyRoles;
        }

        public async Task<List<string>> InflateStudyDataByName(List<UserData> userData)
        {
            //Init & get all sites/roles once
            var errors = new List<string>();
            var studyRoles = await _studyRoleService.GetAll();
            var studySites = await _siteRepository.GetAllSites();

            //Iterate data
            foreach (var user in userData)
            {
                var validStudyUserRoles = new List<StudyUserRole>();
                foreach (var studyUserRole in user.StudyUserRoles)
                {
                    //Validate role
                    var validRole = studyRoles.SingleOrDefault(s => s.ShortName == studyUserRole.RoleName);
                    if (validRole != null)
                    {
                        studyUserRole.StudyRoleId = validRole.Id;
                    }
                    else
                    {
                        errors.Add($"Could not find a StudyRole with the name '{studyUserRole.RoleName}'");
                    }

                    //Validate site
                    var validSite = string.IsNullOrEmpty(studyUserRole.SiteNumber)
                        ? studySites.SingleOrDefault(s => s.Name == studyUserRole.SiteName)
                        : studySites.SingleOrDefault(s => s.SiteNumber == studyUserRole.SiteNumber);

                    if (validSite != null)
                    {
                        studyUserRole.SiteId = validSite.Id;
                        validStudyUserRoles.Add(studyUserRole);
                    }
                    else if (studyUserRole.SiteName == "*")
                    {
                        validStudyUserRoles.AddRange(await GetStudyUserRolesForAllSites(studyUserRole.StudyUserId,
                            studyUserRole.StudyRoleId));
                    }
                    else
                    {
                        errors.Add(
                            $"Could not find a Site with the {(studyUserRole.SiteNumber != null ? "number '" + studyUserRole.SiteNumber : "name '" + studyUserRole.SiteName)}'");
                    }
                }

                user.StudyUserRoles = validStudyUserRoles;
            }

            return errors.Distinct().ToList();
        }

        private async Task<List<StudyUserRole>> GetStudyUserRolesForAllSites(Guid studyUserId, Guid studyRoleId)
        {
            var studyUserRoles = new List<StudyUserRole>();
            foreach (var site in await _siteRepository.GetAllSites())
            {
                studyUserRoles.Add(new StudyUserRole
                {
                    StudyUserId = studyUserId,
                    StudyRoleId = studyRoleId,
                    SiteId = site.Id
                });
            }

            return studyUserRoles;
        }
    }
}