using System;
using System.Collections.Generic;
using YPrime.Core.BusinessLayer.Models;

namespace YPrime.eCOA.DTOLibrary
{
    [Serializable]
    public class QuestionAnswerDto : DtoBase
    {
        public QuestionModel Question { get; set; }
        public List<AnswerDto> Answers { get; set; }
    }
}