using System.Threading.Tasks;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.BusinessLayer.Interfaces
{
    public interface IDataCopyRepository
    {
        Task<StudyPortalConfigDataDto> GetStudyPortalConfigData();

        Task UpdateStudyPortalConfigData(StudyPortalConfigDataDto studyConfig);
    }
}
