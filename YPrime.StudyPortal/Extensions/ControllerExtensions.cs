using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using YPrime.Core.BusinessLayer.Models;

namespace YPrime.StudyPortal.Extensions
{
    public static class ControllerExtensions
    {
        public static SelectList GetPatientStatusTypesSelectList(
            this Controller controller,
            int selectedValue, 
            IEnumerable<PatientStatusModel> statusTypes,
            bool excludeRemoved = true)
        {
            if (excludeRemoved)
            {
                statusTypes = statusTypes
                    .Where(st => !st.IsRemoved);
            }

            var result = new SelectList(
                statusTypes,
                nameof(PatientStatusModel.Id),
                nameof(PatientStatusModel.Name),
                selectedValue);

            return result;
        }

        public static SelectList GetCareGiverTypeSelectList(
            this Controller controller,
            IEnumerable<CareGiverTypeModel> caregiverTypes)
        {
            
            var result = new SelectList(
                caregiverTypes,
                nameof(CareGiverTypeModel.Id),
                nameof(CareGiverTypeModel.Name));

            return result;
        }
    }
}