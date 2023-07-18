using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using YPrime.Data.Study.Models;

namespace YPrime.BusinessLayer.UnitTests.Repositories.CorrectionRepositoryTests
{
    [TestClass]
    public class GetLocalDateForCorrectionTests : CorrectionRepositoryTestBase
    {
        [TestMethod]
        public void GetLocalDateForCorrectionSiteIdTest()
        {
            var siteReportedTime = DateTimeOffset.UtcNow;
            var expectedTime = siteReportedTime.Date.AddDays(1).AddSeconds(-1);

            var siteId = Guid.NewGuid();
            var patientId = Guid.NewGuid();

            MockSiteRepository
                .Setup(s => s.GetSiteLocalTime(It.IsAny<Guid>()))
                .Returns(siteReportedTime);

            var result = Repository.GetLocalDateForCorrection(
                siteId,
                patientId);

            Assert.AreEqual(expectedTime, result);

            MockSiteRepository
                .Verify(
                    s => s.GetSiteLocalTime(It.Is<Guid>(sid => sid == siteId)), 
                    Times.Once);
        }

        [TestMethod]
        public void GetLocalDateForCorrectionPatientIdTest()
        {
            var siteReportedTime = DateTimeOffset.UtcNow.AddMonths(1).AddDays(-10);
            var expectedTime = siteReportedTime.Date.AddDays(1).AddSeconds(-1);

            var siteId = Guid.Empty;
            var patientId = Guid.NewGuid();
            var patientSiteId = Guid.NewGuid();

            MockSiteRepository
                .Setup(s => s.GetSiteLocalTime(It.IsAny<Guid>()))
                .Returns(siteReportedTime);

            var patient = new Patient
            {
                Id = patientId,
                SiteId = patientSiteId
            };

            var patientDbSet = new FakeDbSet<Patient>
            (
                new List<Patient>
                {
                    patient
                }
            );

            MockContext.Setup(ctx => ctx.Patients)
                .Returns(patientDbSet.Object);

            var result = Repository.GetLocalDateForCorrection(
                siteId,
                patientId);

            Assert.AreEqual(expectedTime, result);

            MockSiteRepository
                .Verify(
                    s => s.GetSiteLocalTime(It.Is<Guid>(sid => sid == patientSiteId)),
                    Times.Once);
        }
    }
}
