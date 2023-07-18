using System;

namespace YPrime.eCOA.DTOLibrary.ViewModel
{
    public class SentEmailViewModel
    {
        public Guid Id { get; set; }

        public string Subject { get; set; }

        public string EmailContentType { get; set; }

        public string SiteName { get; set; }

        public string DateSent { get; set; }
        public DateTimeOffset DateSentOffset { get; set; }

        public string LinkHTML { get; set; }
    }
}