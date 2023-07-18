using System;
using System.ComponentModel.DataAnnotations;

namespace YPrime.eCOA.DTOLibrary
{
    [Serializable]
    public class SecurityQuestionDto
    {
        public Guid Id { get; set; }

        [StringLength(255)] public string TranslationKey { get; set; }
    }
}