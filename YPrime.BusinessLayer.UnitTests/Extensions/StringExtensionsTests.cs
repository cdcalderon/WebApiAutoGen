using Microsoft.VisualStudio.TestTools.UnitTesting;
using YPrime.BusinessLayer.Extensions;

namespace YPrime.BusinessLayer.UnitTests.Extensions
{
    [TestClass]
    public class StringExtensionTests
    {
        [TestMethod]
        public void StringExtensionIsNumericPositiveTest()
        {
            const string TestValue = "apu;-9 dfa+";
            const bool ExpectedResult = true;

            var result = TestValue.ContainsNumbers();

            Assert.AreEqual(ExpectedResult, result);
        }

        [TestMethod]
        public void StringExtensionIsNumericNegativeTest()
        {
            const string TestValue = "nvads,.fod   fdasilu(*#";
            const bool ExpectedResult = false;

            var result = TestValue.ContainsNumbers();

            Assert.AreEqual(ExpectedResult, result);
        }

        [TestMethod]
        public void StringExtensionIsNumericEmptyTest()
        {
            var TestValue = string.Empty;
            const bool ExpectedResult = false;

            var result = TestValue.ContainsNumbers();

            Assert.AreEqual(ExpectedResult, result);
        }

        [TestMethod]
        public void StringExtensionIsNumericNullTest()
        {
            const string TestValue = null;
            const bool ExpectedResult = false;

            var result = TestValue.ContainsNumbers();

            Assert.AreEqual(ExpectedResult, result);
        }


        [TestMethod]
        public void StringExtensionsHasValidNumericValueNoMantissaTest()
        {
            const int minValue = 0;
            const int maxValue = 999;
            const bool expectedResult = true;

            const string testValue = "500";

            var result = testValue.HasValidNumericValue(null, minValue, maxValue);

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void StringExtensionsHasValidNumericValueWithMantissaTest()
        {
            const int minValue = 0;
            const int maxValue = 999;
            const int maxLength = 2;
            const bool expectedResult = true;

            const string testValue = "49.26";

            var result = testValue.HasValidNumericValue(maxLength, minValue, maxValue);

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void StringExtensionsHasValidNumericValueMantissaNotAllowedTest()
        {
            const int minValue = 0;
            const int maxValue = 999;
            const int maxLength = 0;
            const bool expectedResult = false;

            const string testValue = "123.45";

            var result = testValue.HasValidNumericValue(maxLength, minValue, maxValue);

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void StringExtensionsHasNumericValueMantissaValueRequiredTest()
        {
            const int minValue = 0;
            const int maxValue = 999;
            const int maxLength = 2;
            const bool expectedResult = false;

            const string testValue = "123";

            var result = testValue.HasValidNumericValue(maxLength, minValue, maxValue);

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void StringExtensionsHasValidNumericValueNoMinTest()
        {
            var minValue = (int?) null;
            const int maxValue = 999;
            const int maxLength = 0;
            const bool expectedResult = true;

            const string testValue = "2";

            var result = testValue.HasValidNumericValue(maxLength, minValue, maxValue);

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void StringExtensionsHasValidNumericValueNoMaxTest()
        {
            const int minValue = 100;
            var maxValue = (int?) null;
            const int maxLength = 0;
            const bool expectedResult = true;

            const string testValue = "99999";

            var result = testValue.HasValidNumericValue(maxLength, minValue, maxValue);

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void StringExtensionsHasValidNumericValueAboveRangeTest()
        {
            const int minValue = 0;
            const int maxValue = 500;
            const int maxLength = 2;
            const bool expectedResult = false;

            const string testValue = "551.90";

            var result = testValue.HasValidNumericValue(maxLength, minValue, maxValue);

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void StringExtensionsHasValidNumericValueBelowRangeTest()
        {
            const int minValue = 200;
            const int maxValue = 500;
            const int maxLength = 0;
            const bool expectedResult = false;

            const string testValue = "100";

            var result = testValue.HasValidNumericValue(maxLength, minValue, maxValue);

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void StringExtensionsHasValidNumericValueEmptyStringTest()
        {
            const int minValue = 0;
            const int maxValue = 500;
            const int maxLength = 0;
            const bool expectedResult = false;

            const string testValue = "";

            var result = testValue.HasValidNumericValue(maxLength, minValue, maxValue);

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void StringExtensionsHasValidNumericValueEmptyCharacteristicTest()
        {
            const int minValue = 0;
            const int maxValue = 1;
            const int maxLength = 3;
            const bool expectedResult = true;

            const string testValue = ".501";

            var result = testValue.HasValidNumericValue(maxLength, minValue, maxValue);

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void StringExtensionsHasValidNumericValueMantissaTrailingZeroTest()
        {
            const int minValue = 0;
            const int maxValue = 5;
            const int maxLength = 3;
            const bool expectedResult = true;

            const string testValue = ".50100";

            var result = testValue.HasValidNumericValue(maxLength, minValue, maxValue);

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void StringExtensionsHasValidNumericValueMantissaTooLongTest()
        {
            const int minValue = 0;
            const int maxValue = 5;
            const int maxLength = 2;
            const bool expectedResult = false;

            const string testValue = ".614";

            var result = testValue.HasValidNumericValue(maxLength, minValue, maxValue);

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void StringExtensionsHasValidNumericValueMantissaAllZeroTest()
        {
            const int minValue = 0;
            const int maxValue = 5;
            const int maxLength = 3;
            const bool expectedResult = true;

            const string testValue = "2.0000";

            var result = testValue.HasValidNumericValue(maxLength, minValue, maxValue);

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void StringExtensionsHasValidNumericValueMantissaPreceededByZerosTest()
        {
            const int minValue = 0;
            const int maxValue = 5;
            const int maxLength = 3;
            const bool expectedResult = false;

            const string testValue = "2.00000005";

            var result = testValue.HasValidNumericValue(maxLength, minValue, maxValue);

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void StringExtensionsHasMaxMantissaValueTest()
        {
            const int minValue = 0;
            const int maxValue = 100;
            const int maxLength = 2;
            const bool expectedResult = true;

            const string testValue = "99.99";

            var result = testValue.HasValidNumericValue(maxLength, minValue, maxValue);

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void StringExtensionsHasValidNumericLengthTest()
        {
            const int maxValue = 22;
            const bool expectedResult = true;

            const string testValue = "11";

            var result = testValue.HasValidNumericLength(maxValue);

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void StringExtensionsInValidNumericLengthTest()
        {
            const int maxValue = 2;
            const bool expectedResult = false;

            const string testValue = "11";

            var result = testValue.HasValidNumericLength(maxValue);

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void StringExtensionsHasValidNumericLengthWithMantissaTest()
        {
            const int maxValue = 22;
            const bool expectedResult = true;

            const string testValue = "11.234";

            var result = testValue.HasValidNumericLength(maxValue);

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void StringExtensionsInValidNumericLengthWithMantissaTest()
        {
            const int maxValue = 22;
            const bool expectedResult = false;

            const string testValue = "1.234";

            var result = testValue.HasValidNumericLength(maxValue);

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void StringExtensionsHasValidNumericLengthNoMaxValueTest()
        {
            int? maxValue = null;
            const bool expectedResult = true;

            const string testValue = "11";

            var result = testValue.HasValidNumericLength(maxValue);

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void StringExtensionsHasValidNumericLengthNoValueTest()
        {
            const int maxValue = 22;
            const bool expectedResult = true;

            const string testValue = "";

            var result = testValue.HasValidNumericLength(maxValue);

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void TestHasValidNumericValue_WithMantissaAllowedAndNumberEndingInDecimal_ReturnsFalse()
        {
            const int minValue = 0;
            const int maxValue = 999;
            const int maxLength = 2;

            const string testValue = "650.";

            var result = testValue.HasValidNumericValue(maxLength, minValue, maxValue);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestHasValidNumericValue_WithMantissaNotAllowedAndNumberEndingInDecimal_ReturnsFalse()
        {
            const int minValue = 0;
            const int maxValue = 999;
            const int maxLength = 0;

            const string testValue = "650.";

            var result = testValue.HasValidNumericValue(maxLength, minValue, maxValue);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void StringContainsOrdinalIgnoreExactMatch_ReturnsTrue()
        {
            var source = "This is a Test";
            var value = "This is a Test";

            var result = source.Contains(value, System.StringComparison.OrdinalIgnoreCase);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void StringContainsOrdinalIgnorePartialExactMatch_ReturnsTrue()
        {
            var source = "This is a Test";
            var value = "Test";

            var result = source.Contains(value, System.StringComparison.OrdinalIgnoreCase);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void StringContainsOrdinalIgnorePartialMatchWithDifferentCasing_ReturnsTrue()
        {
            var source = "This is a Test";
            var value = "tesT";

            var result = source.Contains(value, System.StringComparison.OrdinalIgnoreCase);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void StringContainsOrdinalPartialMatchWithDifferentCasing_ReturnsFalse()
        {
            var source = "This is a Test";
            var value = "tesT";

            var result = source.Contains(value, System.StringComparison.Ordinal);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void StringContainsOrdinalIgnoreMultiWordPartialMatchWithDifferentCasing_ReturnsTrue()
        {
            var source = "This is a Test";
            var value = "A tesT";

            var result = source.Contains(value, System.StringComparison.OrdinalIgnoreCase);

            Assert.IsTrue(result);
        }
    }
}