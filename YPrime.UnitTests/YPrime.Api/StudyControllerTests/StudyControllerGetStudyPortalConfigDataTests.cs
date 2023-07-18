using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Results;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.UnitTests.YPrime.Api.StudyControllerTests
{
    [TestClass]
    public class StudyControllerGetStudyPortalConfigDataTests : StudyControllerTestBase
    {
        [TestInitialize]
        public void TestInitialize()
        {
            BaseInitialize();
        }

        [TestMethod]
        public async Task GetStudyPortalConfigDataTest()
        {
            var testData = new StudyPortalConfigDataDto
            {
                ReportStudyRoles = new List<ReportStudyRole>
                {
                    new ReportStudyRole
                    {
                        ReportId = Guid.NewGuid(),
                        ReportName = Guid.NewGuid().ToString()
                    }
                }
            };

            MockDataCopyRepository
                .Setup(r => r.GetStudyPortalConfigData())
                .ReturnsAsync(testData);

            var controller = GetController();

            var responseResult = await controller.GetStudyPortalConfigData();

            Assert.IsNotNull(responseResult);
            Assert.IsInstanceOfType(responseResult, typeof(OkNegotiatedContentResult<StudyPortalConfigDataDto>));

            var unwrappedResult = responseResult as OkNegotiatedContentResult<StudyPortalConfigDataDto>;

            var result = unwrappedResult.Content;

            Assert.AreEqual(testData.ReportStudyRoles.Count, result.ReportStudyRoles.Count);

            foreach (var resultRole in result.ReportStudyRoles)
            {
                var matchingData = testData.ReportStudyRoles.FirstOrDefault(
                    td =>
                        td.ReportId == resultRole.ReportId &&
                        td.ReportName == resultRole.ReportName &&
                        td.StudyRoleId == resultRole.StudyRoleId);

                Assert.IsNotNull(matchingData);
            }
        }
    }
}
