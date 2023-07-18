using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using YPrime.BusinessLayer.Factories;
using YPrime.BusinessLayer.Interfaces;
using YPrime.BusinessLayer.PowerBi;
using YPrime.Core.BusinessLayer.Interfaces;

namespace YPrime.BusinessLayer.UnitTests.Factories.KeyVaultBasedContextFactoryTests
{
    [TestClass]
    public class KeyVaultBasedContextFactoryTests
    {
        private Mock<IKeyVaultService> _mockKeyVaultService;
        private Mock<IServiceSettings> _mockServiceSettings;
        private IKeyVaultBasedContextFactory _keyVaultBasedContextFactory;

        private Guid studyId = Guid.NewGuid();
        private string keyVaultKey;
        private const string ApiUrl = "https://api.powerbi.com";
        private const string ApplicationId = "4453c30b-93d6-4abb-9e25-23dca87c8323";
        private const string WorkspaceId = "f3172be8-57ef-4c11-a9e1-8c63d026c8a0";
        private const string TenantId = "ad795b81-33c9-4682-9f78-388a0e98489a";
        private const string ApplicationSecret = "ThisIsNotARealSecret";
        private const string AuthorityUrl = "https://login.microsoftonline.com/organizations/";
        private const string ResourceUrl = "https://analysis.windows.net/powerbi/api/.default";

        private const string keyVaultResponse = "{\"ApiUrl\":\"https://api.powerbi.com\",\"ApplicationId\":\"4453c30b-93d6-4abb-9e25-23dca87c8323\",\"ApplicationSecret\":\"ThisIsNotARealSecret\",\"AuthorityUrl\":\"https://login.microsoftonline.com/organizations/\",\"ResourceUrl\":\"https://analysis.windows.net/powerbi/api/.default\",\"TenantId\":\"ad795b81-33c9-4682-9f78-388a0e98489a\",\"WorkspaceId\":\"f3172be8-57ef-4c11-a9e1-8c63d026c8a0\",\"ExcludedTerms\":null,\"ExcludedPrefix\":null}";

        [TestInitialize]
        public void InitializeTests()
        {
            _mockKeyVaultService = new Mock<IKeyVaultService>();
            _mockServiceSettings = new Mock<IServiceSettings>();

            _mockServiceSettings
                .Setup(q => q.StudyId)
                .Returns(studyId);

            keyVaultKey = $"PowerBiContext-{studyId}";
        }

        [TestMethod]
        public async Task GetCurrentContext_ReturnsNewContext()
        {
            _mockKeyVaultService
                .Setup(q => q.GetSecretValueFromKey(keyVaultKey))
                .ReturnsAsync(keyVaultResponse);
            
            _keyVaultBasedContextFactory = new KeyVaultBasedContextFactory(_mockKeyVaultService.Object, _mockServiceSettings.Object);

            var biContext = await _keyVaultBasedContextFactory.GetCurrentContext<PowerBiContext>();

            Assert.IsNotNull(biContext);
            Assert.AreEqual(biContext.ApiUrl, ApiUrl);
            Assert.AreEqual(biContext.ResourceUrl, ResourceUrl);
            Assert.AreEqual(biContext.AuthorityUrl, AuthorityUrl);
            Assert.AreEqual(biContext.ApplicationSecret, ApplicationSecret);
            Assert.AreEqual(biContext.ApplicationId, ApplicationId);
            Assert.AreEqual(biContext.WorkspaceId.ToString(), WorkspaceId);
            Assert.AreEqual(biContext.TenantId, TenantId);

            _mockServiceSettings.Verify(q => q.StudyId, Times.Once());
            _mockKeyVaultService.Verify(q => q.GetSecretValueFromKey(keyVaultKey), Times.Once);
        }

        [TestMethod]
        public async Task GetCurrentContext_ReturnsExistingContext()
        {
            _mockKeyVaultService
                .Setup(q => q.GetSecretValueFromKey(keyVaultKey))
                .ReturnsAsync(keyVaultResponse);

            _keyVaultBasedContextFactory = new KeyVaultBasedContextFactory(_mockKeyVaultService.Object, _mockServiceSettings.Object);

            var biContext = await _keyVaultBasedContextFactory.GetCurrentContext<PowerBiContext>();
            var secondBiContext = await _keyVaultBasedContextFactory.GetCurrentContext<PowerBiContext>();

            _mockServiceSettings.Verify(q => q.StudyId, Times.Once());
            _mockKeyVaultService.Verify(q => q.GetSecretValueFromKey(keyVaultKey), Times.Once);
        }
    }
}
