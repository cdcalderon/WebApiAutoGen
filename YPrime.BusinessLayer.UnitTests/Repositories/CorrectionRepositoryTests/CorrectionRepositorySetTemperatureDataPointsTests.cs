using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YPrime.Core.BusinessLayer.Constants;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study.Models;
using YPrime.eCOA.Utilities.Helpers;

namespace YPrime.BusinessLayer.UnitTests.Repositories.CorrectionRepositoryTests
{
    [TestClass]
    public class CorrectionRepositorySetTemperatureDataPointsTests : CorrectionRepositoryTestBase
    {
        private const string MaxValueCelsius = "32.5";
        private const string MaxValueFahrenheit = "90.5";

        private const string MinValueCelsius = "26.5";
        private const string MinValueFahrenheit = "79.7";

        [TestMethod]
        [DataRow("30.0", "86.0℉", "30.5", "86.9℉", false, false, DisplayName = "Values Unmodified Fahrenheit")]
        [DataRow("30.0", "30.0℃", "30.5", "30.5℃", true, false, DisplayName = "Values Unmodified Celsius")]
        public void SetTemperatureDataPoints_VerifyTests(string oldDataPoint, string oldDisplayValue, string newDataPoint, string newDisplayValue, bool useMetric, bool preserveUnits)
        {
            var testCorrection = CreateDataCorrection(oldDataPoint, oldDisplayValue, newDataPoint, newDisplayValue);
            var testQuestion = CreateQuestionModel(preserveUnits);

            Repository.SetTemperatureDataPoints(testQuestion, testCorrection, useMetric, Constants.CorrectionStateInternal.Verify);

            Assert.AreEqual(testCorrection.OldDisplayValue, oldDisplayValue);
            Assert.AreEqual(testCorrection.OldDataPoint, oldDataPoint);
            Assert.AreEqual(testCorrection.NewDisplayValue, newDisplayValue);
            Assert.AreEqual(testCorrection.NewDataPoint, newDataPoint);
        }

        [TestMethod]
        [DataRow("86.0", "86.0", "87.0", "87.0", false, false, DisplayName = "Don't Preserve Fahrenheit")]
        [DataRow("30.0", "30.0", "30.5", "30.5", true, false, DisplayName = "Don't Preserve Celsius")]
        [DataRow("86.0", "86.0", "87.0", "87.0", false, true, DisplayName = "Preserve Fahrenheit")]
        [DataRow("30.0", "30.0", "30.5", "30.5", true, true, DisplayName = "Preserve Celsius")]
        [DataRow("86.0", "86.0", "87.0", "87.0", false, null, DisplayName = "Legacy Fahrenheit")]
        [DataRow("30.0", "30.0", "30.5", "30.5", true, null, DisplayName = "Legacy Celsius")]
        public void SetTemperatureDataPoints_CreateTests(string oldDataPoint, string oldDisplayValue, string newDataPoint, string newDisplayValue, bool useMetric, bool? preserveUnits)
        {
            var testCorrection = CreateDataCorrection(oldDataPoint, oldDisplayValue, newDataPoint, newDisplayValue);
            var testQuestion = CreateQuestionModel(preserveUnits);

            Repository.SetTemperatureDataPoints(testQuestion, testCorrection, useMetric, Constants.CorrectionStateInternal.Create);

            var displaySuffix = useMetric ? Temperature.DegreesCelsius : Temperature.DegreesFahrenheit;

            Assert.AreEqual(testCorrection.OldDisplayValue, oldDisplayValue + displaySuffix);
            Assert.AreEqual(testCorrection.OldDataPoint, oldDataPoint);
            Assert.AreEqual(testCorrection.NewDisplayValue, newDisplayValue + displaySuffix);
            Assert.AreEqual(testCorrection.NewDataPoint, newDataPoint);
        }

        [TestMethod]
        [DataRow("86.0", "86.0", "87.0", "87.0", false, false, DisplayName = "Don't Preserve Fahrenheit")]
        [DataRow("30.0", "30.0", "30.5", "30.5", true, false, DisplayName = "Don't Preserve Celsius")]
        [DataRow("86.0", "86.0", "87.0", "87.0", false, true, DisplayName = "Preserve Fahrenheit")]
        [DataRow("30.0", "30.0", "30.5", "30.5", true, true, DisplayName = "Preserve Celsius")]
        [DataRow("86.0", "86.0", "87.0", "87.0", false, null, DisplayName = "Legacy Fahrenheit")]
        [DataRow("30.0", "30.0", "30.5", "30.5", true, null, DisplayName = "Legacy Celsius")]
        public void SetTemperatureDataPoints_SaveTests(string oldDataPoint, string oldDisplayValue, string newDataPoint, string newDisplayValue, bool useMetric, bool? preserveUnits)
        {
            var testCorrection = CreateDataCorrection(oldDataPoint, oldDisplayValue, newDataPoint, newDisplayValue);
            var testQuestion = CreateQuestionModel(preserveUnits);

            Repository.SetTemperatureDataPoints(testQuestion, testCorrection, useMetric, Constants.CorrectionStateInternal.Save);

            var displaySuffix = useMetric ? Temperature.DegreesCelsius : Temperature.DegreesFahrenheit;

            var convertedOldDataPoint = ConvertValueForPersistence(oldDataPoint, useMetric, preserveUnits ?? false);
            var convertedNewDataPoint = ConvertValueForPersistence(newDataPoint, useMetric, preserveUnits ?? false);

            string storageSuffix;

            if (preserveUnits == null)
            {
                storageSuffix = string.Empty;
            }
            else
            {
                storageSuffix = string.Format("{0}",
                    useMetric || !(preserveUnits.HasValue && preserveUnits.Value)
                    ? Temperature.Celsius
                    : Temperature.Fahrenheit);
            }

            Assert.AreEqual(testCorrection.OldDisplayValue, oldDisplayValue + displaySuffix);
            Assert.AreEqual(testCorrection.OldDataPoint, convertedOldDataPoint + storageSuffix);
            Assert.AreEqual(testCorrection.NewDisplayValue, newDisplayValue + displaySuffix);
            Assert.AreEqual(testCorrection.NewDataPoint, convertedNewDataPoint + storageSuffix);
        }

        private static string ConvertValueForPersistence(string oldDataPoint, bool useMetric, bool preserveUnits)
        {
            return useMetric || preserveUnits
                ? oldDataPoint
                : TemperatureConversionHelper
                    .ToCelsius(oldDataPoint)
                    .ToString(TemperatureControlConstants.NonPreserveSaveFormat);
        }

        private static QuestionModel CreateQuestionModel(bool? preserveUnits)
        {
            var isLegacy = preserveUnits == null;

            var temperatureMinMax = isLegacy
                ? null
                : new TemperatureConfigModel
                {
                    MaxCelsius = MaxValueCelsius,
                    MaxFahrenheit = MaxValueFahrenheit,
                    MinCelsius = MinValueCelsius,
                    MinFahrenheit = MinValueFahrenheit,
                    PreserveUnits = preserveUnits
                };

            return new QuestionModel
            {
                Id = Guid.NewGuid(),
                QuestionSettings = new QuestionSettingsModel
                {
                    MinValue = isLegacy ? MinValueCelsius : null,
                    MaxValue = isLegacy ? MaxValueCelsius : null,
                    TemperatureMinMax = temperatureMinMax
                },
            };
        }

        private static CorrectionApprovalData CreateDataCorrection(string oldDataPoint, string oldDisplayValue, string newDataPoint, string newDisplayValue)
        {
            return new CorrectionApprovalData
            {
                Id = Guid.NewGuid(),
                OldDataPoint = oldDataPoint,
                OldDisplayValue = oldDisplayValue,
                NewDataPoint = newDataPoint,
                NewDisplayValue = newDisplayValue
            };
        }
    }
}
