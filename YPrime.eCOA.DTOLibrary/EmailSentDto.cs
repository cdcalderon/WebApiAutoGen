using System;
using YPrime.Data.Study.Models;

namespace YPrime.eCOA.DTOLibrary
{
    [Serializable]
    public class EmailSentDto : DtoBase
    {
        public Guid Id { get; set; }
        public string Body { get; set; }
        public string Subject { get; set; }
        public DateTimeOffset DateSent { get; set; }
        public Guid EmailContentId { get; set; }
        public Guid? StudyUserId { get; set; }
        public string EmailContentType { get; set; }
        public string SiteName { get; set; }
        public virtual EmailContent EmailCOntent { get; set; }
        public virtual StudyUser StudyUser { get; set; }
        public string Recipients { get; set; }
    }
}