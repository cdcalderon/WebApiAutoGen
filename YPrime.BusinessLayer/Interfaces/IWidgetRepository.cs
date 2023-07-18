using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YPrime.Data.Study.Models;

namespace YPrime.BusinessLayer.Interfaces
{
    public interface IWidgetRepository
    {
        Task<List<Widget>> GetDashboardWidgets(string cultureCode, Guid roleId, Guid userId);
        Task<List<Widget>> GetWidgets(string cultureCode, Guid roleId, Guid userId);
        Task<Widget> GetWidget(Guid id, string cultureCode, Guid? userId = null);
        void SaveStudyUserWidgets(Guid userId, List<StudyUserWidget> studyUserWidgets);
        void SaveStudyRoleWidgets(Guid roleId, List<StudyRoleWidget> studyRoleWidgets);
    }
}