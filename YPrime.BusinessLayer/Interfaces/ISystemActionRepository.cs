using System.Collections.Generic;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.BusinessLayer.Interfaces
{
    public interface ISystemActionRepository
    {
        List<SystemActionDto> GetAllSystemActions();
        void SaveActionList(Dictionary<string, dynamic> ActionList);
    }
}