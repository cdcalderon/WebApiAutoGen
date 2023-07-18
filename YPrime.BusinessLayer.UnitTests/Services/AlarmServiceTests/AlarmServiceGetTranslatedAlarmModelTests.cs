using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YPrime.Core.BusinessLayer.Models;

namespace YPrime.BusinessLayer.UnitTests.Services.AlarmServiceTests
{
    [TestClass]
    public class AlarmServiceGetTranslatedAlarmModelTests : AlarmServiceTestBase
    {
        [TestMethod]
        public async Task GetTranslatedAlarmModelTest()
        {
            var testAlarmId = Guid.NewGuid();

            var expectedResult = JsonConvert.DeserializeObject<AlarmModel>(TranslatedAlarmResponse);
            var expectedAddress = $"{ExpectedBaseEndpointAddress}/{testAlarmId}/{Config.Defaults.Languages.English.Id}";

            SetupHttpFactory(System.Net.HttpStatusCode.OK, TranslatedAlarmResponse);

            var service = GetService();

            var result = await service.GetTranslatedAlarmModel(testAlarmId);

            result.Should().BeEquivalentTo(expectedResult);

            MockHttpFactory.Verify(
                f => f.CreateClient(It.IsAny<string>()),
                Times.Once);

            MockClientHandler
                .Protected()
                .Verify(
                    nameof(HttpClient.SendAsync),
                    Times.Exactly(1),
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>());

            Assert.AreEqual(expectedAddress, PassedInRequestMessage.RequestUri.AbsoluteUri);
            Assert.AreEqual(HttpMethod.Get, PassedInRequestMessage.Method);
        }
    }
}
