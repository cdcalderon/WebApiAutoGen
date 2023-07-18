using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using YPrime.Config.Enums;
using YPrime.Core.BusinessLayer.Extensions;
using YPrime.Core.BusinessLayer.Models;

namespace YPrime.BusinessLayer.UnitTests.Extensions
{
    [TestClass]
    public class SubjectInformationModelExtensionsTests
    {
        [TestMethod]
        public void SubjectInformationModelGetMinimumValueTest()
        {
            const int expectedValue = 1234;

            var model = new SubjectInformationModel
            {
                Min = expectedValue.ToString()
            };

            var result = model.GetMininumValue();

            Assert.AreEqual(expectedValue, result);
        }

        [TestMethod]
        public void SubjectInformationModelGetMaximumValueTest()
        {
            const int expectedValue = 543;

            var model = new SubjectInformationModel
            {
                Max = expectedValue.ToString()
            };

            var result = model.GetMaximumValue();

            Assert.AreEqual(expectedValue, result);
        }

        [TestMethod]
        public void SubjectInformationModelParseNullMinMaxTest()
        {
            var model = new SubjectInformationModel();

            var minResult = model.GetMaximumValue();
            var maxResult = model.GetMaximumValue();

            Assert.IsNull(minResult);
            Assert.IsNull(maxResult);
        }

        [TestMethod]
        public void SubjectInformationModelsDateFormatMonthYearValidTest()
        {
            const string dateFormat = "MMM/yyyy";
            const string testValue = "Sep/2019";
            const bool expectedResult = true;

            var testModel = new SubjectInformationModel
            {
                DateFormat = dateFormat
            };

            var result = testModel.HasValidDateFormat(testValue);

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void SubjectInformationModelDateFormatMonthYearInvalidTest()
        {
            const string dateFormat = "MMM/yyyy";
            const string testValue = "08/Sep/2019";
            const bool expectedResult = false;

            var testModel = new SubjectInformationModel
            {
                DateFormat = dateFormat
            };

            var result = testModel.HasValidDateFormat(testValue);

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void SubjectInformationModelDateFormatMonthValidTest()
        {
            const string dateFormat = "MMM";
            const string testValue = "Nov";
            const bool expectedResult = true;

            var testModel = new SubjectInformationModel
            {
                DateFormat = dateFormat
            };

            var result = testModel.HasValidDateFormat(testValue);

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void SubjectInformationModelDateFormatMonthInvalidTest()
        {
            const string dateFormat = "MMM";
            const string testValue = "Bad";
            const bool expectedResult = false;

            var testModel = new SubjectInformationModel
            {
                DateFormat = dateFormat
            };

            var result = testModel.HasValidDateFormat(testValue);

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void SubjectInformationModelDateFormatYearValidTest()
        {
            const string dateFormat = "yyyy";
            const string testValue = "2019";
            const bool expectedResult = true;

            var testModel = new SubjectInformationModel
            {
                DateFormat = dateFormat
            };

            var result = testModel.HasValidDateFormat(testValue);

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void SubjectInformationModelDateFormatYearInvalidTest()
        {
            const string dateFormat = "yyyy";
            const string testValue = "201";
            const bool expectedResult = false;

            var testModel = new SubjectInformationModel
            {
                DateFormat = dateFormat
            };

            var result = testModel.HasValidDateFormat(testValue);

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void SubjectInformationModelDateFormatFullDateValidTest()
        {
            const string dateFormat = "dd/MM/yyyy";
            const string testValue = "18/12/2005";
            const bool expectedResult = true;

            var testModel = new SubjectInformationModel
            {
                DateFormat = dateFormat
            };

            var result = testModel.HasValidDateFormat(testValue);

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void SubjectInformationModelDateFormatFullMonthValidTest()
        {
            const string dateFormat = "dd/MMMM/yyyy";
            const string testValue = "21/July/2010";
            const bool expectedResult = true;

            var testModel = new SubjectInformationModel
            {
                DateFormat = dateFormat
            };

            var result = testModel.HasValidDateFormat(testValue);

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void SubjectInformationModelDateFormatFullDateInvalidTest()
        {
            const string dateFormat = "dd/MM/yyyy";
            const string testValue = "30/02/2014";
            const bool expectedResult = false;

            var testModel = new SubjectInformationModel
            {
                DateFormat = dateFormat
            };

            var result = testModel.HasValidDateFormat(testValue);

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void SubjectInformationModelDateFormatNoFormatTest()
        {
            const string testValue = "01/January/1994";
            const bool expectedResult = true;

            var testModel = new SubjectInformationModel();

            var result = testModel.HasValidDateFormat(testValue);

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void SubjectInformationModelMinMaxInRangeTest()
        {
            const string dateFormat = "dd/MM/yyyy";
            var testValue = DateTime.Now.AddDays(30).AddMonths(1).ToString(dateFormat, CultureInfo.InvariantCulture);
            const bool expectedResult = true;
            const string minDate = "{{TODAY-0d:0m:1y}}";
            const string maxDate = "{{TODAY+0d:1m:1y}}";

            var testModel = new SubjectInformationModel
            {
                DateFormat = dateFormat,
                Min = minDate,
                Max = maxDate
            };

            var result = testModel.HasDateWithinRangeOfMinMax(testValue, DateTimeOffset.Now);

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void SubjectInformationModelMinMaxBelowRangeTest()
        {
            const string dateFormat = "dd/MM/yyyy";
            var testValue = DateTime.Now.AddYears(-101).ToString(dateFormat, CultureInfo.InvariantCulture);
            const bool expectedResult = false;
            const string minDate = "{{TODAY-0d:0m:1y}}";
            const string maxDate = "{{TODAY+0d:1m:1y}}";

            var testModel = new SubjectInformationModel
            {
                DateFormat = dateFormat,
                Min = minDate,
                Max = maxDate
            };

            var result = testModel.HasDateWithinRangeOfMinMax(testValue, DateTimeOffset.Now);

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void SubjectInformationModelMinMaxAboveRangeTest()
        {
            const string dateFormat = "dd/MM/yyyy";
            var testValue = DateTime.Now.AddYears(101).ToString(dateFormat, CultureInfo.InvariantCulture);
            const bool expectedResult = false;
            const string minDate = "{{TODAY-5d:1m:0y}}";
            const string maxDate = "{{TODAY+5d:1m:0y}}";

            var testModel = new SubjectInformationModel
            {
                DateFormat = dateFormat,
                Min = minDate,
                Max = maxDate
            };

            var result = testModel.HasDateWithinRangeOfMinMax(testValue, DateTimeOffset.Now);

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void SubjectInformationModelsMinMaxAboveRangeTodayTest()
        {
            const string dateFormat = "dd/MM/yyyy";
            var testValue = DateTime.Now.AddDays(1).ToString(dateFormat, CultureInfo.InvariantCulture);
            const bool expectedResult = false;
            const string minDate = "{{TODAY-5d:1m:0y}}";
            const string maxDate = "{{TODAY-0d:0m:0y}}";

            var testModel = new SubjectInformationModel
            {
                DateFormat = dateFormat,
                Min = minDate,
                Max = maxDate
            };

            var result = testModel.HasDateWithinRangeOfMinMax(testValue, DateTimeOffset.Now);

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void SubjectInformationModelMinMaxInvalidValueTest()
        {
            const string testValue = "this-is-not-a-valid-date";
            const bool expectedResult = false;
            const string minDate = "{{TODAY-0d:0m:1y}}";
            const string maxDate = "{{TODAY+0d:1m:1y}}";
            const string dateFormat = "dd/MM/yyyy";

            var testModel = new SubjectInformationModel
            {
                DateFormat = dateFormat,
                Min = minDate,
                Max = maxDate
            };

            var result = testModel.HasDateWithinRangeOfMinMax(testValue, DateTimeOffset.Now);

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void SubjectInformationModelMonthFormatMinMaxTest()
        {
            const string dateFormat = "MMMM/yyyy";

            var testValue = DateTime.Now.ToString(dateFormat, CultureInfo.InvariantCulture);

            const bool expectedResult = true;
            const string minDate = "{{TODAY-0d:0m:0y}}";
            const string maxDate = "{{TODAY+0d:0m:0y}}";

            var testModel = new SubjectInformationModel
            {
                DateFormat = dateFormat,
                Min = minDate,
                Max = maxDate
            };

            var result = testModel.HasDateWithinRangeOfMinMax(testValue, DateTimeOffset.Now);

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void SubjectInformationModelMonthFormatMinMaxOutOfRangeTest()
        {
            const string dateFormat = "MMMM/yyyy";

            var testValue = DateTime.Now.AddMonths(-1).ToString(dateFormat, CultureInfo.InvariantCulture);

            const bool expectedResult = false;
            const string minDate = "{{TODAY-0d:0m:0y}}";
            const string maxDate = "{{TODAY+0d:0m:0y}}";

            var testModel = new SubjectInformationModel
            {
                DateFormat = dateFormat,
                Min = minDate,
                Max = maxDate
            };

            var result = testModel.HasDateWithinRangeOfMinMax(testValue, DateTimeOffset.Now);

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void SubjectInformationModelYearFormatMinMaxTest()
        {
            const string dateFormat = "yyyy";

            var testValue = DateTime.Now.ToString(dateFormat, CultureInfo.InvariantCulture);

            const bool expectedResult = true;
            const string minDate = "{{TODAY-0d:0m:0y}}";
            const string maxDate = "{{TODAY+0d:0m:0y}}";

            var testModel = new SubjectInformationModel
            {
                DateFormat = dateFormat,
                Min = minDate,
                Max = maxDate
            };

            var result = testModel.HasDateWithinRangeOfMinMax(testValue, DateTimeOffset.Now);

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void SubjectInformationModelYearFormatMinMaxOutOfRangeTest()
        {
            const string dateFormat = "yyyy";

            var testValue = DateTime.Now.AddYears(-1).ToString(dateFormat, CultureInfo.InvariantCulture);

            const bool expectedResult = false;
            const string minDate = "{{TODAY-0d:0m:0y}}";
            const string maxDate = "{{TODAY+0d:0m:0y}}";

            var testModel = new SubjectInformationModel
            {
                DateFormat = dateFormat,
                Min = minDate,
                Max = maxDate
            };

            var result = testModel.HasDateWithinRangeOfMinMax(testValue, DateTimeOffset.Now);

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void SubjectInformationModelYearFormatYearAndMonthInRangeTest()
        {
            const string dateFormat = "MMMM/yyyy";

            var testValue = new DateTime(2020, 9, 1).ToString(dateFormat, CultureInfo.InvariantCulture);

            const bool expectedResult = true;
            const string minDate = "{{1/March/2015-0d:0m:0y}}";
            const string maxDate = "{{30/March/2025+0d:0m:0y}}";

            var testModel = new SubjectInformationModel
            {
                DateFormat = dateFormat,
                Min = minDate,
                Max = maxDate
            };

            var result = testModel.HasDateWithinRangeOfMinMax(testValue, DateTimeOffset.Now);

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void SubjectInforatmionModelGetDataTypeDateTest()
        {
            var expectedDataType = DataType.DateAttribute;

            var testModel = new SubjectInformationModel
            {
                ChoiceType = expectedDataType.DisplayName
            };

            var result = testModel.GetDataType();

            Assert.AreEqual(expectedDataType, result);
        }

        [TestMethod]
        public void SubjectInforatmionModelGetDataTypeChoiceTest()
        {
            var expectedDataType = DataType.ChoicesAttribute;

            var testModel = new SubjectInformationModel
            {
                ChoiceType = expectedDataType.DisplayName
            };

            var result = testModel.GetDataType();

            Assert.AreEqual(expectedDataType, result);
        }

        [TestMethod]
        public void SubjectInforatmionModelGetDataTypeNumberTest()
        {
            var expectedDataType = DataType.NumberAttribute;

            var testModel = new SubjectInformationModel
            {
                ChoiceType = expectedDataType.DisplayName
            };

            var result = testModel.GetDataType();

            Assert.AreEqual(expectedDataType, result);
        }

        [TestMethod]
        public void SubjectInforatmionModelGetDataTypeDecimalTest()
        {
            var baseType = DataType.NumberAttribute;
            var expectedDataType = DataType.DecimalNumberAttribute;

            var testModel = new SubjectInformationModel
            {
                ChoiceType = baseType.DisplayName,
                Decimal = 1
            };

            var result = testModel.GetDataType();

            Assert.AreEqual(expectedDataType, result);
        }

        [TestMethod]
        public void SubjectInforatmionModelGetDataTypeTextTest()
        {
            var expectedDataType = DataType.TextAttribute;

            var testModel = new SubjectInformationModel
            {
                ChoiceType = expectedDataType.DisplayName
            };

            var result = testModel.GetDataType();

            Assert.AreEqual(expectedDataType, result);
        }

        [TestMethod]
        public void SubjectInforatmionModelGetDataTypeLettersOnlyTest()
        {
            var baseType = DataType.TextAttribute;
            var expectedDataType = DataType.LettersOnlyAttribute;

            var testModel = new SubjectInformationModel
            {
                ChoiceType = baseType.DisplayName,
                DisableNumeric = true
            };

            var result = testModel.GetDataType();

            Assert.AreEqual(expectedDataType, result);
        }

        [TestMethod]
        public void SubjectInforatmionModelGetDataTypeNullTest()
        {
            var testModel = new SubjectInformationModel();

            var result = testModel.GetDataType();

            Assert.IsNull(result);
        }
    }
}
