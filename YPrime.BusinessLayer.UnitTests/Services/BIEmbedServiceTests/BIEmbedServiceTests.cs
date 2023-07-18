using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.SessionState;
using Microsoft.PowerBI.Api.Models;
using Moq;
using NUnit.Framework;
using YPrime.BusinessLayer.Interfaces;
using YPrime.BusinessLayer.PowerBi;
using YPrime.BusinessLayer.Services;
using YPrime.BusinessLayer.Session;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.BusinessLayer.UnitTests.Services.BIEmbedServiceTests
{
    public class BIEmbedServiceTests
    {
        private readonly Guid _testStudyId = Guid.NewGuid();
        private readonly Guid _testSponsorId = Guid.NewGuid();

        private readonly Mock<IRoleRepository> _mockRoleRepository = new Mock<IRoleRepository>();
        private readonly Mock<IKeyVaultBasedContextFactory> _mockKeyVaultContextFactory = new Mock<IKeyVaultBasedContextFactory>();
        private readonly Mock<IServiceSettings> _mockServiceSettings = new Mock<IServiceSettings>();
        private readonly Mock<Microsoft.PowerBI.Api.Models.Reports> _mockReports = new Mock<Microsoft.PowerBI.Api.Models.Reports>();

        #region Analytics

        private const int TotalNumberOfAnalytics = 3;
        private const int ZeroAnalytics = 0;

        private readonly AnalyticsReference _analyticOne = new AnalyticsReference
        {
            DisplayName = "Display Name One"
        };

        private readonly AnalyticsReference _analyticTwo = new AnalyticsReference
        {
            DisplayName = "Display Name Two"
        };

        private readonly AnalyticsReference _analyticThree = new AnalyticsReference
        {
            DisplayName = "Display Name Three"
        };

        private readonly PowerBiContext testPowerBiContext = new PowerBiContext(
            "testApiUrl", 
            Guid.NewGuid().ToString(), 
            "testApplicationString", 
            "testAuthorityUrl", 
            "testResourceUrl", 
            Guid.NewGuid().ToString(), 
            Guid.NewGuid());

    #endregion Analytics

    #region Reports

        private readonly Report _reportOneValid = new Report
        {
            Id = Guid.NewGuid(),
            Name = "Display Name One"
        };

        private readonly Report _reportTwoValid = new Report
        {
            Id = Guid.NewGuid(),
            Name = "Display Name Two"
        };

        private readonly Report _reportThreeValid = new Report
        {
            Id = Guid.NewGuid(),
            Name = "Display Name Three"
        };

        private readonly Report _reportOneInvalidWithPrefixPrefixAtBeginningAndEnd = new Report
        {
            Id = Guid.NewGuid(),
            Name = "vDisplay Name Onev"
        };

        private readonly Report _reportTwoInvalidPrefixAtBeginning = new Report
        {
            Id = Guid.NewGuid(),
            Name = "vDisplay Name Two"
        };

        private readonly Report _reportThreeInvalidExcludedTermMetrics = new Report
        {
            Id = Guid.NewGuid(),
            Name = "Display Name Three metrics"
        };

        private readonly Report _reportFourInvalidExcludedTermYPrimeDevReports = new Report
        {
            Id = Guid.NewGuid(),
            Name = "Display Name Four YPrime Dev Reports"
        };

        private readonly Report _reportFiveInvalidExcludedTermDataset = new Report
        {
            Id = Guid.NewGuid(),
            Name = "Display dataset Name Five "
        };

        private readonly Report _reportSixInvalidPrefixAndAllExcludedTerms = new Report
        {
            Id = Guid.NewGuid(),
            Name = "vReport Name Six metrics YPrime Dev dataset"
        };

        #endregion Reports

        protected YPrimeSession YPrimeSession = new YPrimeSession();

        protected void SetupSession()
        {
            HttpContext.Current = GetMockedHttpContext();
        }

        protected HttpContext GetMockedHttpContext()
        {
            var user = new Mock<IPrincipal>();
            var identity = new Mock<IIdentity>();

            user.Setup(ctx => ctx.Identity).Returns(identity.Object);
            identity.Setup(id => id.IsAuthenticated).Returns(true);
            identity.Setup(id => id.Name).Returns("test");

            var context = new HttpContext(
                new HttpRequest(string.Empty, "http://tempuri.org", string.Empty),
                new HttpResponse(new StringWriter()));

            context.User = user.Object;

            var sessionContainer = new HttpSessionStateContainer(
                "id",
                new SessionStateItemCollection(),
                new HttpStaticObjectsCollection(),
                10,
                true,
                HttpCookieMode.AutoDetect,
                SessionStateMode.InProc,
                false);

            SessionStateUtility.AddHttpSessionStateToContext(context, sessionContainer);

            context.Session.Add("YPrimeSessionInstance", YPrimeSession);

            return context;
        }

        [SetUp]
        public void Setup()
        {
            _mockReports.Object.Value = new List<Report>()
                {
                    _reportOneValid, _reportTwoValid, _reportThreeValid,
                    _reportOneInvalidWithPrefixPrefixAtBeginningAndEnd, _reportTwoInvalidPrefixAtBeginning,
                    _reportThreeInvalidExcludedTermMetrics, _reportFourInvalidExcludedTermYPrimeDevReports,
                    _reportFiveInvalidExcludedTermDataset, _reportSixInvalidPrefixAndAllExcludedTerms
                };

            var roleId = Guid.NewGuid();

            var studyRoleDtos = new List<StudyRoleDto>
            {
                new StudyRoleDto
                    {Id = roleId}
            };
            _mockRoleRepository
                .Setup(x => x.GetUserRoles(It.IsAny<Guid>()))
                .ReturnsAsync(studyRoleDtos);

            _mockKeyVaultContextFactory
                .Setup(x => x.GetCurrentContext<PowerBiContext>())
                .ReturnsAsync(testPowerBiContext);

            _mockServiceSettings
                .SetupProperty(s => s.StudyId)
                .SetReturnsDefault(_testStudyId);

            _mockServiceSettings
                .SetupProperty(s => s.SponsorId)
                .SetReturnsDefault(_testSponsorId);

            YPrimeSession.CurrentUser = new StudyUserDto
            {
                Id = Guid.NewGuid()
            };
            SetupSession();
        }

        [Test]
        public async Task RoleFilterReportsReturnsValidReportsAssignedToRole()
        {
            var embedService = new BIEmbedService(_mockServiceSettings.Object, _mockRoleRepository.Object, _mockKeyVaultContextFactory.Object);

            _mockRoleRepository.Setup(q => q.GetAllAnalyticsReferencesByRole(It.IsAny<Guid>())).Returns(Task.FromResult(new List<AnalyticsReference> { _analyticOne, _analyticTwo, _analyticThree }));

            var filteredReports = await embedService.FilterReportsFromRole(_mockReports.Object.Value);

            Assert.AreEqual(TotalNumberOfAnalytics, filteredReports.Count());
        }

        [Test]
        public async Task RoleFilterReportsReturnsNoReports()
        {
            var embedService = new BIEmbedService(_mockServiceSettings.Object, _mockRoleRepository.Object, _mockKeyVaultContextFactory.Object);

            _mockRoleRepository.Setup(q => q.GetAllAnalyticsReferencesByRole(It.IsAny<Guid>())).Returns(Task.FromResult(new List<AnalyticsReference> { }));

            var filteredReports = await embedService.FilterReportsFromRole(_mockReports.Object.Value);

            Assert.AreEqual(ZeroAnalytics, filteredReports.Count());
        }
    }
}