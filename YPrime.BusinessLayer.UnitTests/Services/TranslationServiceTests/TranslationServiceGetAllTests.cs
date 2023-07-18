using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace YPrime.BusinessLayer.UnitTests.Services.TranslationServiceTests
{
    [TestClass]
    public class TranslationServiceGetAllTests : TranslationServiceTestBase
    {
        [TestMethod]
        public void GetAllTest()
        {
            SetupHttpFactory(System.Net.HttpStatusCode.OK, EnglishLanguageResponse);

            var service = GetService();

            service
                .Invoking(async s => await s.GetAll())
                .Should()
                .Throw<NotImplementedException>();
        }
    }
}
