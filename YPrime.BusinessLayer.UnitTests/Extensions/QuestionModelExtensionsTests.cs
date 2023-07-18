using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using YPrime.BusinessLayer.Extensions;
using YPrime.Config.Enums;
using YPrime.Core.BusinessLayer.Extensions;
using YPrime.Core.BusinessLayer.Models;


namespace YPrime.BusinessLayer.UnitTests.Extensions
{
    [TestClass]
    public class QuestionModelExtensionsTests
    {
        private const string MaxValueCelsius = "32.5";
        private const string MaxValueFahrenheit = "90.5";

        private const string MinValueCelsius = "26.5";
        private const string MinValueFahrenheit = "79.7";

        [TestMethod]
        public void GetInputFieldTypeTest()
        {
            var question = new QuestionModel();
            question.QuestionType = InputFieldType.NumberSpinner.Id;

            var result = question.GetInputFieldType();

            Assert.AreEqual(InputFieldType.NumberSpinner, result);
        }

        [TestMethod]
        public void IsNumericValueQuestionTypeNumberSpinnerTest()
        {
            var question = new QuestionModel();
            question.QuestionType = InputFieldType.NumberSpinner.Id;

            Assert.IsTrue(question.IsNumericValueQuestionType());
        }

        [TestMethod]
        public void IsNumericValueQuestionTypeVasTest()
        {
            var question = new QuestionModel();
            question.QuestionType = InputFieldType.VAS.Id;

            Assert.IsTrue(question.IsNumericValueQuestionType());
        }

        [TestMethod]
        public void IsNumericValueQuestionTypeEqTest()
        {
            var question = new QuestionModel();
            question.QuestionType = InputFieldType.EQ5D5L.Id;

            Assert.IsTrue(question.IsNumericValueQuestionType());
        }

        [TestMethod]
        public void IsNumericValueQuestionTypeFalseTest()
        {
            var question = new QuestionModel();
            question.QuestionType = InputFieldType.Date.Id;

            Assert.IsFalse(question.IsNumericValueQuestionType());
        }

        [TestMethod]
        public void IsHotspotQuestionTypeTrueTest()
        {
            var question = new QuestionModel();
            question.QuestionType = InputFieldType.HotSpotSingleSelect.Id;

            Assert.IsTrue(question.IsHotspotQuestionType());
        }

        [TestMethod]
        public void IsHotspotQuestionTypeFalseTest()
        {
            var question = new QuestionModel();
            question.QuestionType = InputFieldType.TextArea.Id;

            Assert.IsFalse(question.IsHotspotQuestionType());
        }

        [TestMethod]
        public void SortChoicesTest()
        {
            var choice1 = new QuestionChoiceModel
            {
                Id = Guid.NewGuid(),
                Sequence = 1
            };

            var choice2 = new QuestionChoiceModel
            {
                Id = Guid.NewGuid(),
                Sequence = 2
            };


            var question = new QuestionModel
            {
                Choices = new List<QuestionChoiceModel>
                {
                    choice2,
                    choice1
                }
            };

            question.SortChoices();

            var firstOrderedId = question.Choices.First().Id;
            var secondOrderedId = question.Choices.Skip(1).First().Id;

            Assert.AreEqual(choice1.Id, firstOrderedId);
            Assert.AreEqual(choice2.Id, secondOrderedId);
        }

        [TestMethod]
        public void SortChoicesNullTest()
        {
            var question = new QuestionModel
            {
                Choices = null
            };

            question.SortChoices();

            Assert.IsNull(question.Choices);
        }

        [TestMethod]
        public void GetDecimalValue()
        {
            int? decimalValue = 1;
            var question = new QuestionModel();
            question.QuestionSettings = new QuestionSettingsModel();
            question.QuestionSettings.DecimalValue = decimalValue.ToString();

            Assert.AreEqual(question.GetDecimalValue(), decimalValue);
        }

        [TestMethod]
        public void CheckToSeeIfTemperatureControl()
        {
            var question = new QuestionModel();
            question.InputFieldTypeId = InputFieldType.TemperatureSpinner.Id;
            Assert.IsTrue(question.IsTemperatureQuestionType());

            question.InputFieldTypeId = InputFieldType.TextArea.Id;
            Assert.IsFalse(question.IsTemperatureQuestionType());
        }

        [TestMethod]
        public void GetStepValue()
        {
            int? decimalValue = 1;
            var question = new QuestionModel();
            question.QuestionSettings = new QuestionSettingsModel();
            question.QuestionSettings.DecimalValue = decimalValue.ToString();

            Assert.AreEqual(question.GetStepValue(), "0.1");
        }

        [TestMethod]
        public void GetStepValueForNone()
        {
            var question = new QuestionModel();
            question.QuestionSettings = new QuestionSettingsModel();
            question.QuestionSettings.DecimalValue = "";

            Assert.AreEqual(question.GetStepValue(), "1");
        }

        [TestMethod]
        public void GetStepValueFor3()
        {
            int? decimalValue = 3;
            var question = new QuestionModel();
            question.QuestionSettings = new QuestionSettingsModel();
            question.QuestionSettings.DecimalValue = decimalValue.ToString();

            Assert.AreEqual(question.GetStepValue(), "0.001");
        }

        [TestMethod]
        public void GetMinValue_NonTemperatureReturnsMinValue()
        {
            var minValue = "25";

            var question = new QuestionModel
            {
                QuestionType = InputFieldType.NumberSpinner.Id,
                QuestionSettings = new QuestionSettingsModel { MinValue = minValue }
            };

            var result = question.GetMinValue(true);

            Assert.AreEqual(minValue, result);
        }

        [TestMethod]
        public void GetMinValue_TemperatureQuestionLegacyModeReturnsMinValue()
        {
            var question = CreateTemperatureQuestion(true);

            var result = question.GetMinValue(true);

            Assert.AreEqual(MinValueCelsius, result);
        }

        [TestMethod]
        public void GetMinValue_TemperatureQuestionLegacyModeNonMetricReturnsMinValue()
        {
            var question = CreateTemperatureQuestion(true);

            var result = question.GetMinValue(false);

            Assert.AreEqual(MinValueFahrenheit, result);
        }

        [TestMethod]
        public void GetMinValue_TemperatureQuestionUpdatedModeReturnsMinValue()
        {
            var question = CreateTemperatureQuestion(false);

            var result = question.GetMinValue(true);

            Assert.AreEqual(MinValueCelsius, result);
        }

        [TestMethod]
        public void GetMinValue_TemperatureQuestionUpdatedModeNonMetricReturnsMinValue()
        {
            var question = CreateTemperatureQuestion(false);

            var result = question.GetMinValue(false);

            Assert.AreEqual(MinValueFahrenheit, result);
        }

        [TestMethod]
        public void GetMaxValue_NonTemperatureReturnsMaxValue()
        {
            var maxValue = "100";

            var question = new QuestionModel
            {
                QuestionType = InputFieldType.NumberSpinner.Id,
                QuestionSettings = new QuestionSettingsModel { MaxValue = maxValue }
            };

            var result = question.GetMaxValue(true);

            Assert.AreEqual(maxValue, result);
        }

        [TestMethod]
        public void GetMaxValue_TemperatureQuestionLegacyModeReturnsMaxValue()
        {
            var question = CreateTemperatureQuestion(true);

            var result = question.GetMaxValue(true);

            Assert.AreEqual(MaxValueCelsius, result);
        }

        [TestMethod]
        public void GetMaxValue_TemperatureQuestionLegacyModeNonMetricReturnsMaxValue()
        {
            var question = CreateTemperatureQuestion(true);

            var result = question.GetMaxValue(false);

            Assert.AreEqual(MaxValueFahrenheit, result);
        }

        [TestMethod]
        public void GetMaxValue_TemperatureQuestionUpdatedModeReturnsMaxValue()
        {
            var question = CreateTemperatureQuestion(false);

            var result = question.GetMaxValue(true);

            Assert.AreEqual(MaxValueCelsius, result);
        }

        [TestMethod]
        public void GetMaxValue_TemperatureQuestionUpdatedModeNonMetricReturnsMaxValue()
        {
            var question = CreateTemperatureQuestion(false);

            var result = question.GetMaxValue(false);

            Assert.AreEqual(MaxValueFahrenheit, result);
        }

        private static QuestionModel CreateTemperatureQuestion(bool useLegacyMode)
        {
            var questionModel = new QuestionModel
            {
                QuestionType = InputFieldType.TemperatureSpinner.Id
            };

            if (useLegacyMode)
            {
                questionModel.QuestionSettings = new QuestionSettingsModel
                {
                    MinValue = MinValueCelsius,
                    MaxValue = MaxValueCelsius
                };
            }
            else
            {
                questionModel.QuestionSettings = new QuestionSettingsModel
                {
                    TemperatureMinMax = new TemperatureConfigModel
                    {
                        MinFahrenheit = MinValueFahrenheit,
                        MaxFahrenheit = MaxValueFahrenheit,
                        MinCelsius = MinValueCelsius,
                        MaxCelsius = MaxValueCelsius,
                        PreserveUnits = true
                    }
                };
            }

            return questionModel;
        }
    }
}
