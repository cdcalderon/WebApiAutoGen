using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using YPrime.Core.BusinessLayer.Extensions;
using YPrime.Core.BusinessLayer.Models;

namespace YPrime.BusinessLayer.UnitTests.Extensions
{
    [TestClass]
    public class StudySettingModelExtensionsTests
    {
        [TestMethod]
        public void StudySettingModelExtensionsGetIntValueTest()
        {
            const int expectedValue = 8;

            var model = new StudyCustomModel
            {
                Value = expectedValue.ToString()
            };

            var result = model.GetIntValue();

            Assert.AreEqual(expectedValue, result);
        }

        [TestMethod]
        public void StudySettingModelExtensionsGetIntValueNonIntTest()
        {
            var model = new StudyCustomModel
            {
                Value = "xyz"
            };

            model
                .Invoking(m => m.GetIntValue())
                .Should()
                .Throw<FormatException>();
        }

        [TestMethod]
        public void StudySettingModelExtensionsGetStringValueTest()
        {
            var expectedValue = Guid.NewGuid();

            var model = new StudyCustomModel
            {
                Value = expectedValue.ToString()
            };

            var result = model.GetGuidValue();

            Assert.AreEqual(expectedValue, result);
        }

        [TestMethod]
        public void StudySettingModelExtensionsGetBoolValueTest()
        {
            var trueModel = new StudyCustomModel
            {
                Value = "True"
            };

            var falseModel = new StudyCustomModel
            {
                Value = "false"
            };

            var oneModel = new StudyCustomModel
            {
                Value = "1"
            };

            var zeroModel = new StudyCustomModel
            {
                Value = "0"
            };

            var invalidModel = new StudyCustomModel
            {
                Value = "xyz"
            };

            var trueResult = trueModel.GetBoolValue();
            var falseResult = falseModel.GetBoolValue();
            var oneResult = oneModel.GetBoolValue();
            var zeroResult = zeroModel.GetBoolValue();
            var invalidResult = zeroModel.GetBoolValue();

            Assert.IsTrue(trueResult);
            Assert.IsFalse(falseResult);
            Assert.IsTrue(oneResult);
            Assert.IsFalse(zeroResult);
            Assert.IsFalse(invalidResult);
        }
    }
}
