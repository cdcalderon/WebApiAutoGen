using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using YPrime.BusinessLayer.Interfaces;
using YPrime.BusinessLayer.Models;
using YPrime.BusinessLayer.UnitTests;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;
using YPrime.StudyPortal.Controllers;
using YPrime.UnitTests.YPrime.PatientPortal.Tests.Controllers;

namespace YPrime.UnitTests.YPrime.StudyPortal.Tests.Controllers.AnalyticsControllerTests
{
    [TestClass]
    public class AnalyticsControllerTests : BaseControllerTest
    {
        protected AnalyticsController Controller;
        protected Mock<ISessionService> _sessionService;
        protected Mock<IBIEmbedService> _embedService;
        protected Mock<ISiteRepository> _siteRepository;

        [TestInitialize]
        public void TestInitialize()
        {
            _sessionService = new Mock<ISessionService>();
            _embedService = new Mock<IBIEmbedService>();
            _siteRepository = new Mock<ISiteRepository>();

            Controller = new AnalyticsController(_sessionService.Object, _embedService.Object, _siteRepository.Object);

            _embedService.Setup(es => es.GetAnalyticsInWorkspace())
                .ReturnsAsync(new List<ExternalReport> {
                    new ExternalReport
                    {
                        Report = new Microsoft.PowerBI.Api.Models.Report(Guid.NewGuid()),
                        IsSponsorReport = false
                    }
                    
                });

            _embedService.Setup(es => es.GetEmbedConfig(It.IsAny<Guid>(), It.IsAny<string[]>()))
               .ReturnsAsync(new EmbedConfig());
        }

        [TestMethod]
        public async Task GetAnalyticsInWorkspaceWhenCalled_WillProduceReportType()
        {
            var result = await Controller.GetAnalyticsInWorkspace();

            var json = new JavaScriptSerializer().Deserialize<IEnumerable<Microsoft.PowerBI.Api.Models.Report>>(result);
            YAssert.IsType<IEnumerable<Microsoft.PowerBI.Api.Models.Report>>(json);
        }

        [TestMethod]
        public async Task GetEmbedConfigWhenCalled_WillProduceEmbedConfig()
        {
            var result = await Controller.GetEmbedConfig(Guid.NewGuid().ToString());

            var json = new JavaScriptSerializer().Deserialize<EmbedConfig>(result);
            YAssert.IsType<EmbedConfig>(json);
        }
    }
}
