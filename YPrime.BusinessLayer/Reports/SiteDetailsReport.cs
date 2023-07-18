using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YPrime.BusinessLayer.Interfaces;
using YPrime.BusinessLayer.Reports;
using YPrime.BusinessLayer.Reports.Factory;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Data.Study;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.Reports.Reports
{
    public class SiteDetailsReport : IReport
    {
        private const string ReportDateFormat = "dd-MMM-yyyy";

        private readonly Dictionary<string, string> _columns = new Dictionary<string, string>
        {
            {"CountryName", "Country"},
            {"SiteNumber", "Site Number"},
            {"Investigator", "Investigator Name"},
            {"IsActive", "Site Status"},
            {"ScreeningFirst", "Date of First Screening"},
            {"DateOfActivation", "Date of Activation"},
            {"DateOfDeactivation", "Date of Deactivation"},
            {"DateOfReactivation", "Date of Reactivation"}
        };

        private readonly IStudyDbContext _db;
        private readonly ICountryService _countryService;
        private readonly IVisitService _visitService;
        private readonly IUserRepository _userRepository;

        public SiteDetailsReport(
            IStudyDbContext db,
            ICountryService countryService,
            IVisitService visitService,
            IUserRepository userRepository)
        {
            _db = db;
            _countryService = countryService;
            _visitService = visitService;
            _userRepository = userRepository;
        }

        public Dictionary<string, string> GetColumnHeadings() => _columns;

        public async Task<List<ReportDataDto>> GetGridData(Dictionary<string, object> parameters, Guid userId)
        {
            var data = await GetData(userId);
            return data.ToList().ToReportData(_columns.Keys.ToList());
        }

        public Task<ChartDataObject> GetReportChartData(Dictionary<string, object> parameters, Guid userId) => Task.FromResult<ChartDataObject>(null);

        private async Task<List<SiteDetailsDto>> GetData(Guid userId)
        {
            var countries = await _countryService.GetAll();
            var visits = await _visitService.GetAll();

            var screeningVisit = visits.FirstOrDefault(x => x.VisitOrder == 2)?.Id ?? Guid.Empty;

            var result = _userRepository.GetValidSitesForUser(userId)
                .Select(s => new SiteDetailsDto
                {
                    CountryName = countries.FirstOrDefault(c => c.Id == s.CountryId)?.Name,
                    SiteNumber = s.SiteNumber,
                    Investigator = s.Investigator,
                    IsActive = s.IsActive ? "Active" : "Inactive",
                    ScreeningFirst =
                        FormatDate(
                            s.Patient.SelectMany(p => p.PatientVisits)
                                .Where(pv => pv.VisitId == screeningVisit && pv.VisitDate != null)
                                .OrderBy(pv => pv.VisitDate).Select(pv => pv.VisitDate).FirstOrDefault(),
                            ReportDateFormat),
                    RandomizationFirst = string.Empty,
                    DateOfActivation =
                        FormatDate(
                            s.SiteActiveHistory
                                .Where(sa => !sa.Previous && sa.Current)
                                .OrderBy(sa => sa.ChangeDate)
                                .FirstOrDefault()
                                ?.ChangeDate, ReportDateFormat),
                    DateOfDeactivation =
                        FormatDate(
                            s.SiteActiveHistory
                                .Where(sa => sa.Previous && !sa.Current)
                                .OrderByDescending(sa => sa.ChangeDate)
                                .FirstOrDefault()
                                ?.ChangeDate, ReportDateFormat),
                    DateOfReactivation =
                            s.SiteActiveHistory.Count(sa => !sa.Previous && sa.Current) > 1
                                ? FormatDate(s.SiteActiveHistory
                                    .Where(sa => !sa.Previous && sa.Current)
                                    .OrderByDescending(sa => sa.ChangeDate)
                                    .FirstOrDefault()
                                    ?.ChangeDate, ReportDateFormat)
                                : null
                })
                .OrderBy(u => u.SiteNumber)
                .ToList();

            return result;
        }

        private static string FormatDate(DateTimeOffset? value, string dateFormatPattern)
        {
            string result = null;

            if (value.HasValue && value.Value != default)
            {
                result = value.Value.ToString(dateFormatPattern);
            }

            return result;
        }

        private class SiteDetailsDto
        {
            public string CountryName { get; set; }

            public string SiteNumber { get; set; }

            public string Investigator { get; set; }

            public string IsActive { get; set; }

            public string ScreeningFirst { get; set; }

            public string RandomizationFirst { get; set; }

            public string DateOfActivation { get; set; }

            public string DateOfDeactivation { get; set; }

            public string DateOfReactivation { get; set; }
        }
    }
}