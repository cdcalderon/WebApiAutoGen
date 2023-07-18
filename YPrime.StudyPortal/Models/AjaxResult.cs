using System;
using System.Runtime.Serialization;

namespace YPrime.StudyPortal.Models
{
    [Serializable]
    public class AjaxResult
    {
        [DataMember] public bool Success { get; set; }

        [DataMember] public string Message { get; set; }

        [DataMember] public string MessageTitle { get; set; }

        [DataMember] public string RedirectUrl { get; set; }

        [DataMember] public string JsonData { get; set; }

        [DataMember]
        public bool IsDefaultAjaxObject =>
            //use this in the js helper to determine if it is a standard ajax post
            true;
    }
}