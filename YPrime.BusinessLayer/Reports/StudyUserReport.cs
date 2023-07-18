using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using YPrime.BusinessLayer.Interfaces;
using YPrime.BusinessLayer.Reports.Factory;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Data.Study;
using YPrime.eCOA.DTOLibrary;

[assembly: InternalsVisibleTo("YPrime.UnitTests")]

namespace YPrime.Reports.Reports
{
    public class StudyUserReport : IReport
    {
        private readonly IStudyDbContext _db;
        internal IAuthenticationUserRepository _userRepository { get; set; }
        internal IStudyRoleService _studyRoleService { get; set; }

        public StudyUserReport(
            IStudyDbContext db,      
            IAuthenticationUserRepository userRepository,
            IStudyRoleService studyRoleService)
        {
            _db = db;
            _userRepository = userRepository;
            _studyRoleService = studyRoleService;
        }

        public Task<ChartDataObject> GetReportChartData(Dictionary<string, object> parameters, Guid userId)
        {
            return Task.FromResult<ChartDataObject>(null);
        }

        public async Task<List<ReportDataDto>> GetGridData(Dictionary<string, object> parameters, Guid userId)
        {
            var data = new List<ReportDataDto>();
            var Sites = await GetReportData(userId);

            foreach (var s in Sites)
            {
                var result = new ReportDataDto();

                result.Row.Add("SiteNumber", s.SiteNumber);
                result.Row.Add("UserName", s.UserName);
                result.Row.Add("FirstName", s.FirstName);
                result.Row.Add("LastName", s.LastName);
                result.Row.Add("UserRole", s.UserRole);
                data.Add(result);
            }

            return data;
        }

        public Dictionary<string, string> GetColumnHeadings()
        {
            return ColumnHeaderMappings();
        }

        public Dictionary<string, string> ColumnHeaderMappings()
        {
            var Rtn = new Dictionary<string, string>();
            Rtn.Add("SiteNumber", "Site");
            Rtn.Add("UserName", "Username");
            Rtn.Add("FirstName", "First Name");
            Rtn.Add("LastName", "Last Name");
            Rtn.Add("UserRole", "Role");
            return Rtn;
        }

        private async Task<IEnumerable<StudyUserReportDTO>> GetReportData(Guid UserId)
        {
            var sites = _db.StudyUserRoles.Where(s => s.StudyUserId == UserId).Select(s => s.Site);
            var userRolesBySite = _db.StudyUserRoles.Where(s => sites.Select(i => i.Id).Contains(s.SiteId)).ToList();
            var userIds = userRolesBySite.Select(s => s.StudyUserId).ToList();
            var studyUsers = (await _userRepository.GetUsersAsync(userIds)).ToList();
            var reportData = new List<StudyUserReportDTO>();
            var allRoles = await _studyRoleService.GetAll();

            foreach (var userRole in userRolesBySite)
            {
                var studyUser = studyUsers.SingleOrDefault(s => s.StudyUser.Id == userRole.StudyUserId);
                var studyRole = allRoles.FirstOrDefault(x => x.Id == userRole.StudyRoleId);
                if (studyUser != null)
                {
                    var dto = new StudyUserReportDTO
                    {
                        UserName = studyUser.StudyUser.UserName,
                        FirstName = studyUser.FirstName,
                        LastName = studyUser.LastName,
                        SiteNumber = userRole.Site.SiteNumber,
                        UserRole = studyRole?.ShortName
                    };

                    reportData.Add(dto);
                }
            }

            reportData = reportData
                .OrderBy(rd => rd.SiteNumber)
                .ThenBy(rd => rd.UserName)
                .ThenBy(rd => rd.LastName)
                .ThenBy(rd => rd.FirstName)
                .ToList();

            return reportData;
        }

        public class StudyUserReportDTO
        {
            public string SiteNumber { get; set; }
            public string UserName { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string UserRole { get; set; }
        }
    }
}