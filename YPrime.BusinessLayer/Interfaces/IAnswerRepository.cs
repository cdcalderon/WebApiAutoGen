using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.BusinessLayer.Interfaces
{
    public interface IAnswerRepository : IBaseRepository
    {
        Task<List<AnswerDto>> GetAnswers(AnswerPropertiesDto answerProperties);

        Task<Image> GetImage(Guid Id);
    }
}