using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace YPrime.BusinessLayer.UnitTests.Services.CorrectionWorkflowServiceTests
{
    [TestClass]
    public class CorrectionWorkflowServiceGetAllTests : CorrectionWorkflowServiceTestBase
    {
        [TestMethod]
        public void GetAllTest()
        {
            SetupHttpFactory(System.Net.HttpStatusCode.OK, "{ }");

            var service = GetService();

            service
                .Invoking(async s => await s.GetAll())
                .Should()
                .Throw<NotImplementedException>();
        }
    }
}
