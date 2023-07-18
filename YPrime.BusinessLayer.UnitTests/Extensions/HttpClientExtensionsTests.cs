using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using YPrime.BusinessLayer.UnitTests.Services;
using YPrime.Core.BusinessLayer.Extensions;

namespace YPrime.BusinessLayer.UnitTests.Extensions
{
    [TestClass]
    public class HttpClientExtensionsTests : ConfigServiceTestBase<object>
    {
        private HttpClient _client;
        public HttpClientExtensionsTests()
            : base(string.Empty)
        {
            
        }

        [TestMethod]
        public void ShouldSetAuthorizationHeaderWithGivenParameters()
        {
            _client = MockHttpFactory.Object.CreateClient("httpClient");
            _client.SetHmacAuthorizationHeader(Guid.NewGuid().ToString(), "test", TestServiceSettings.HMACAuthSharedKey);
            Assert.IsNotNull(_client.DefaultRequestHeaders.Authorization);
        }
    }
}
