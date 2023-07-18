using System;
using System.Threading.Tasks;
using YPrime.Core.BusinessLayer.Models;

namespace YPrime.Core.BusinessLayer.Interfaces
{
    public interface IStudyRoleService : IConfigServiceBase<StudyRoleModel, Guid>
    {
        Task LoadIntoCache(Guid configurationId);
    }
}
