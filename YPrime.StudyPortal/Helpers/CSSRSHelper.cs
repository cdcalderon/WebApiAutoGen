using System.Collections.Generic;
using System.Linq;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.StudyPortal.Helpers
{
    public static class CSSRSHelper
    {
        private const string LTSeverityKey = "LTCSSRSSevereValue";
        private const string SixMSeverityKey = "6MCSSRSSevereValue";
        private const string SLVSeverityKey = "SLVCSSRSSevereValue";

        private static List<string> SeverityKeys = new List<string>() { LTSeverityKey, SixMSeverityKey, SLVSeverityKey };

        private static bool IsCSSRSPlaceholderQuestion(string questionText)
        {
            return SeverityKeys.Any(k => questionText.Contains(k));
        }

        public static void FilterPlaceholderQuestions(DiaryEntryDto diaryEntry)
        {
            if (diaryEntry.Answers.Any(a => CSSRSHelper.IsCSSRSPlaceholderQuestion(a.DisplayQuestion)))
            {
                diaryEntry.Answers = diaryEntry.Answers
                                    .Where(a => !CSSRSHelper.IsCSSRSPlaceholderQuestion(a.DisplayQuestion))
                                    .OrderBy(a => a.Question?.Sequence)
                                    .ToList();
            }  
        }
    }
}