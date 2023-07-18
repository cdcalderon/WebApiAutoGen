using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YPrime.Data.Study.Models;
using YPrime.Data.Study.Models.Models;
using Moq;
using YPrime.BusinessLayer.Repositories;
using YPrime.Data.Study;


namespace YPrime.BusinessLayer.UnitTests.Repositories.SystemSettingRepositoryTests
{
    [TestClass]
    public class SystemSettingGetSystemSettingValueTest : SystemSettingRepositoryTestBase
    { 

        [TestInitialize]
        public void TestInitialize()
        {
            var _dataTestA = new SystemSetting
            {
                Id = 1,
                Name = "testSetting1",
                Value = "test value1"
            };

            var _dataTestB = new SystemSetting
            {
                Id = 2,
                Name = "testSetting2",
                Value = "test value2"
            };

            var _dataSet = new FakeDbSet<SystemSetting>(new[] { _dataTestA, _dataTestB });
            Context.Setup(ctx => ctx.SystemSettings)
                .Returns(_dataSet.Object);
        }

        [TestMethod]
        public void GetSystemSettingValueTest()
        {
            var result = Repository.GetSystemSettingValue("testSetting1");
            Assert.AreEqual("test value1", result);
        }

        [TestMethod]
        public void GetSystemSettingValueNotFoundTest()
        {
            var result = Repository.GetSystemSettingValue("testSetting3");
            Assert.AreEqual(null, result);
        }
    }
}
