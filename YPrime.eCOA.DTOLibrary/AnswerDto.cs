using System;
using System.ComponentModel.DataAnnotations;
using YPrime.Core.BusinessLayer.Models;

namespace YPrime.eCOA.DTOLibrary
{
    [Serializable]
    public class AnswerDto : DtoBase
    {
        public AnswerDto()
        {
            Position = 0;
        }

        public Guid Id { get; set; }

        public Guid? DiaryEntryId { get; set; }

        public Guid QuestionId { get; set; }

        public QuestionModel Question { get; set; }

        public Guid? ChoiceId { get; set; }

        public int? ChoiceSequence { get; set; }

        public string FreeTextAnswer { get; set; }

        public string DisplayQuestion { get; set; }

        public string DisplayAnswer { get; set; }

        public int PageNumber { get; set; }

        public QuestionChoiceModel Choice { get; set; }

        [SkipPropertyCopy] 
        public DiaryEntryDto DiaryEntry { get; set; }

        public bool Checked { get; set; }

        public bool MultipleChoice { get; set; }

        public bool IsRequired { get; set; } = false;

        public int Position { get; set; }

        public string Suffix { get; set; }

        public bool IsCountryMetric { get; set; }

        [Required]
        [Range(typeof(bool), "false", "true", ErrorMessage = "Required")]
        public bool IsAnswered
        {
            get
            {
                bool result;

                if (!IsRequired)
                {
                    result = true;
                }
                else
                {
                    if (MultipleChoice)
                    {
                        result = Checked;
                    }
                    else
                    {
                        result = (ChoiceId.HasValue || !string.IsNullOrEmpty(FreeTextAnswer));
                    }
                }

                return result;
            }
        }

        public string Value
        {
            get
            {
                string result = null;

                if (ChoiceId != null && Checked)
                {
                    result = ChoiceId.ToString();
                }
                else
                {
                    if (!string.IsNullOrEmpty(FreeTextAnswer))
                    {
                        result = FreeTextAnswer;
                    }
                }

                return result;
            }
        }
    }
}