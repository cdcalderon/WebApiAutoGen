using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.PowerBI.Api.Models;
using YPrime.BusinessLayer.Extensions;
using System;
using YPrime.BusinessLayer.Models;

namespace YPrime.BusinessLayer.UnitTests.Extensions
{
    [TestClass]
    public class PowerBiExtensionsTests
    {
        #region Exclusions

        private readonly HashSet<string> _excludedTerms = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "metrics",
            "reports",
            "dataset"
        };

        private readonly IReadOnlyCollection<string> _listOfTermsThatWillNotExcludeAnyReports = new List<string>();

        private const string ExcludedPrefix = "HIDDEN";

        #endregion Exclusions

        private readonly ExternalReport ReportWithOneExcludedTerm = new ExternalReport
        {
            Report = new Report { Id = Guid.NewGuid(), Name = "Internal dataset" },
            IsSponsorReport = false
        };
            
        private readonly ExternalReport ReportWithTwoExcludedTerms = new ExternalReport 
        { 
            Report = new Report { Id = Guid.NewGuid(), Name = "dataset reports" },
            IsSponsorReport = false
        };

        private readonly ExternalReport ReportWithPascalCaseExcludedTerm = new ExternalReport
        {
            Report = new Report { Id = Guid.NewGuid(), Name = "Internal Dataset" },
            IsSponsorReport = false
        };

        private readonly ExternalReport ReportWithNoRestrictions = new ExternalReport
        {
            Report = new Report { Id = Guid.NewGuid(), Name = "Test Report" },
            IsSponsorReport = false
        };


        private readonly ExternalReport ReportWithInvalidPrefix = new ExternalReport
        {
            Report = new Report { Id = Guid.NewGuid(), Name = "HIDDEN Report" },
            IsSponsorReport = false
        };

        [TestMethod]
        public void FilterByExcludedPrefix_ReturnsAllReports()
        {
            var analytics = new List<ExternalReport> 
            { 
                ReportWithOneExcludedTerm, 
                ReportWithTwoExcludedTerms, 
                ReportWithNoRestrictions, 
                ReportWithPascalCaseExcludedTerm 
            };

            var filteredAnalytics = analytics.FilterByExcludedPrefix(ExcludedPrefix);

            Assert.AreEqual(analytics.Count, filteredAnalytics.Count());
        }

        [TestMethod]
        public void FilterByExcludedPrefix_ReturnsValidReports()
        {
            var analytics = new List<ExternalReport>
            {
                ReportWithOneExcludedTerm,
                ReportWithTwoExcludedTerms,
                ReportWithNoRestrictions,
                ReportWithPascalCaseExcludedTerm,
                ReportWithInvalidPrefix
            };

            var filteredAnalytics = analytics.FilterByExcludedPrefix(ExcludedPrefix);

            Assert.AreEqual(4, filteredAnalytics.Count());
        }

        [TestMethod]
        public void FilterByExcludedTerms_ReturnsAllReports()
        {
            var analytics = new List<ExternalReport>
            {
                ReportWithNoRestrictions,
                ReportWithInvalidPrefix
            };

            var filteredAnalytics = analytics.FilterByExcludedTerms(_excludedTerms);

            Assert.AreEqual(2, filteredAnalytics.Count());
        }

        [TestMethod]
        public void FilterByExcludedTerms_ReturnsReportsWithoutTerms()
        {
            var analytics = new List<ExternalReport>
            {
                ReportWithOneExcludedTerm,
                ReportWithTwoExcludedTerms,
                ReportWithPascalCaseExcludedTerm,
                ReportWithNoRestrictions,
                ReportWithInvalidPrefix
            };

            var filteredAnalytics = analytics.FilterByExcludedTerms(_excludedTerms);

            Assert.AreEqual(2, filteredAnalytics.Count());
        }
    }
}
