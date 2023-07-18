using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using YPrime.Config.Enums;
using YPrime.Core.BusinessLayer.Constants;
using YPrime.Core.BusinessLayer.Models;
using YPrime.eCOA.Utilities.Helpers;

namespace YPrime.Core.BusinessLayer.Extensions
{
    public static class QuestionModelExtensions
    {
        public static InputFieldType GetInputFieldType(this QuestionModel model)
        {
            var enumType = InputFieldType
                .FirstOrDefault<InputFieldType>(i => i.Id == model.QuestionType);
            return enumType;
        }

        public static bool IsNumericValueQuestionType(this QuestionModel model)
        {
            var numericQuestionTypes = new List<int>()
            {
                InputFieldType.NumberSpinner.Id,
                InputFieldType.NRS.Id,
                InputFieldType.VAS.Id,
                InputFieldType.EQ5D5L.Id,
                InputFieldType.TemperatureSpinner.Id
            };
            return numericQuestionTypes.Contains(model.QuestionType);
        }

        //TODO:refactor this out for change questionnaire Response in 5.9(mimic paper dcf)
        public static bool IsDecimalValueQuestionType(this QuestionModel model)
        {
            var numericQuestionTypes = new List<int>()
            {
                InputFieldType.NumberSpinner.Id,
                InputFieldType.TemperatureSpinner.Id
            };

            var decimalValue = GetDecimalValue(model);

            if (string.IsNullOrEmpty(decimalValue.ToString()) || decimalValue == 0)
            {
                numericQuestionTypes.Remove(model.InputFieldTypeId);
            }

            return numericQuestionTypes.Contains(model.QuestionType);
        }

        public static bool IsHotspotQuestionType(this QuestionModel model)
        {
            var hotspotQuestionTypes = new List<int>()
            {
                InputFieldType.HotSpotSingleSelect.Id,
                InputFieldType.HotSpotMultipleSelect.Id
            };
            return hotspotQuestionTypes.Contains(model.QuestionType);
        }

        public static bool IsTemperatureQuestionType(this QuestionModel model)
        {
            return model?.QuestionType == InputFieldType.TemperatureSpinner.Id;
        }

        public static bool IsEQ5D5LQuestionType(this QuestionModel model)
        {
            return model?.QuestionType == InputFieldType.EQ5D5L.Id;
        }

        public static void SortChoices(this QuestionModel model)
        {
            if (model == null || model.Choices == null || !model.Choices.Any())
            {
                return;
            }

            model.Choices = model.Choices
                .OrderBy(c => c.Sequence)
                .ToList();
        }

        public static string GetMinValue(this QuestionModel model, bool isMetric)
        {
            var minValue = model.MinValue;

            if (model.IsTemperatureQuestionType())
            {
                // pull from the temperature specific config if available
                var minTempValue = isMetric
                    ? model.TemperatureMinMax?.MinCelsius ?? minValue
                    : model.TemperatureMinMax?.MinFahrenheit ?? TemperatureConversionHelper.ToFahrenheit(minValue).ToString(TemperatureControlConstants.DisplayFormat, CultureInfo.InvariantCulture);

                minValue = Math.Round(
                        Convert.ToSingle(minTempValue, CultureInfo.InvariantCulture),
                        TemperatureControlConstants.DecimalPlaces)
                    .ToString(TemperatureControlConstants.DisplayFormat, CultureInfo.InvariantCulture);
            }

            return minValue;
        }

        public static string GetMaxValue(this QuestionModel model, bool isMetric)
        {
            var maxValue = model.MaxValue;

            if (model.IsTemperatureQuestionType())
            {
                // pull from the temperature specific config if available
                var maxTempValue = isMetric
                    ? model.TemperatureMinMax?.MaxCelsius ?? maxValue
                    : model.TemperatureMinMax?.MaxFahrenheit ?? TemperatureConversionHelper.ToFahrenheit(maxValue).ToString(TemperatureControlConstants.DisplayFormat, CultureInfo.InvariantCulture);

                maxValue = Math.Round(
                        Convert.ToSingle(maxTempValue, CultureInfo.InvariantCulture),
                        TemperatureControlConstants.DecimalPlaces)
                    .ToString(TemperatureControlConstants.DisplayFormat, CultureInfo.InvariantCulture);
            }

            return maxValue;
        }

        public static int? GetDecimalValue(this QuestionModel model)
        {
            int? result = null;

            if (model.IsTemperatureQuestionType())
            {
                result = TemperatureControlConstants.DecimalPlaces;
            }
            else if (int.TryParse(model?.QuestionSettings?.DecimalValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsedValue))
            {
                result = parsedValue;
            }

            return result;
        }

        public static string GetStepValue(this QuestionModel model)
        {
            string result = GetDecimalValue(model).ToString();
            string stepValue = "1";

            bool success = int.TryParse(
                result,
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out var value);

            var noDecimalResult = result == "0" || string.IsNullOrEmpty(result);

            return noDecimalResult || !success ? stepValue : "0." + stepValue.PadLeft(value, '0');
        }
    }
}
