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
    public class SystemSettingAddSystemSettingTest : SystemSettingRepositoryTestBase
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

            var data = new List<SystemSetting> { _dataTestA, _dataTestB };
            var _dataSet = new FakeDbSet<SystemSetting>(data);
            _dataSet.Setup(ds => ds.Add(It.IsAny<SystemSetting>()))
                .Returns<SystemSetting>(systemSetting =>
                {
                    systemSetting.Id = data.Max(d => d.Id) + 1;
                    data.Add(systemSetting);
                    return systemSetting;
                });
            Context.Setup(ctx => ctx.SystemSettings)
                .Returns(_dataSet.Object);
        }

        [TestMethod]
        public void AddSystemSettingTest()
        {
            int expected = 3;
            var setting = new SystemSetting
            {
                Name = "test setting 3",
                Value = "test value 3"
            };

            var actual = Repository.AddSystemSetting(setting);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void AddSystemSettingAlreadyExistsTest()
        {
            int expected = 2;
            var setting = new SystemSetting
            {
                Name = "testSetting2",
                Value = "test value2"
            };

            var actual = Repository.AddSystemSetting(setting);
            Assert.AreEqual(expected, actual);
        }
    }
}
