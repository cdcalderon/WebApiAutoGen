using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.BusinessLayer.Interfaces
{
    public interface IAnalyticsRepository
    {
        Guid AddAnalyticsReference(AnalyticsReference analyticsReference);
        List<AnalyticsReference> GetAllAnalyticsReferences();
        bool RemoveReportByInitialName(string internalName);
        AnalyticsReference UpdateReportName(string internalName, string updatedInternalName, string updatedDisplayName);
    }
}
