using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YPrime.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;
using YPrime.Reports.Reports;

namespace YPrime.BusinessLayer.UnitTests.Reports
{
    [TestClass]
    public class AverageDiaryDurationReportTests
    {
        private const string TestQuestionnaireDisplayName = "Test Questionnaire ABC";
        
        private Mock<IDiaryEntryRepository> MockDiaryEntryRepository = new Mock<IDiaryEntryRepository>();


        private DiaryEntryDto DiaryEntry1;
        private DiaryEntryDto DiaryEntry2;
        private DiaryEntryDto DiaryEntry3;


        [TestInitialize]
        public void TestInitialize()
        {
            MockDiaryEntryRepository = new Mock<IDiaryEntryRepository>();

            DiaryEntry1 = new DiaryEntryDto
            {
                QuestionnaireDisplayName = TestQuestionnaireDisplayName,
                StartedTime = new DateTimeOffset(new DateTime(2020, 1, 1, 9, 30, 0)),
                CompletedTime = new DateTimeOffset(new DateTime(2020, 1, 1, 9, 40, 0)),
            };

            DiaryEntry2 = new DiaryEntryDto
            {
                QuestionnaireDisplayName = TestQuestionnaireDisplayName,
                StartedTime = new DateTimeOffset(new DateTime(2020, 2, 17, 13, 20, 0)),
                CompletedTime = new DateTimeOffset(new DateTime(2020, 2, 17, 13, 40, 0)),
            };

            DiaryEntry3 = new DiaryEntryDto
            {
                QuestionnaireDisplayName = TestQuestionnaireDisplayName,
                StartedTime = new DateTimeOffset(new DateTime(2020, 3, 4, 17, 06, 0)),
                CompletedTime = new DateTimeOffset(new DateTime(2020, 3, 4, 17, 36, 0)),
            };

            MockDiaryEntryRepository
                .Setup(r => r.GetDiaryEntriesInflated(
                    It.Is<Guid?>(qid => qid == null),
                    It.Is<bool>(ia => ia == false),
                    It.Is<bool?>(ib => ib == false),
                    It.Is<Guid?>(pid => pid == null)))
                .ReturnsAsync(new List<DiaryEntryDto>
                {
                    DiaryEntry1,
                    DiaryEntry2,
                    DiaryEntry3
                });
        }

        [TestMethod]
        public async Task GetGridDataTest()
        {
            var report = GetReport();

            var results = await report.GetGridData(
                null, 
                Guid.NewGuid());

            Assert.AreEqual(1, results.Count());

            var result = results.First();

            Assert.AreEqual(TestQuestionnaireDisplayName, result["Questionnaire_Name"]);
            Assert.AreEqual(20.0f, result["Average_Duration"]);
        }

        [TestMethod]
        public void GetColumnHeadingsTest()
        {
            var report = GetReport();

            var result = report.GetColumnHeadings();

            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public async Task GetReportChartDataTest()
        {
            var report = GetReport();

            var result = await report.GetReportChartData(
                null,
                Guid.NewGuid());

            Assert.AreEqual(1, result.XLabels.Count);

            var firstLabel = result.XLabels.First();

            Assert.AreEqual(1.0d, firstLabel.Key);
            Assert.AreEqual(TestQuestionnaireDisplayName, firstLabel.Value);

            Assert.AreEqual(1, result.ChartSeries.Count);

            var firstSeries = result.ChartSeries.First();

            Assert.AreEqual("Questionnaires", firstSeries.SeriesName);
            Assert.AreEqual(1, firstSeries.SeriesDataPoints.Count);

            var firstSeriesDataPoint = firstSeries.SeriesDataPoints.First();

            Assert.AreEqual(1.0f, firstSeriesDataPoint.X);
            Assert.AreEqual(20.0f, firstSeriesDataPoint.Y);
        }

        private AverageDiaryDurationReport GetReport()
        {
            var report = new AverageDiaryDurationReport(
                MockDiaryEntryRepository.Object);

            return report;
        }
    }
}
