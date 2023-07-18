using System;

namespace YPrime.API.Models
{
    public class FileUploadData
    {
        public Guid? DiaryEntryId { get; set; }
        public string Base64DataUrl { get; set; }
        public string FileName { get; set; }
    }
}