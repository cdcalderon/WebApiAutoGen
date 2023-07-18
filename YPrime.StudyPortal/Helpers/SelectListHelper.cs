using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.StudyPortal.Helpers
{
    public static class SelectListHelper
    {
        public static string AllSitesText => "All Sites";
        public static string AllPatientsText => "All Patients";
        public static string AllPatientStatusTypesText => "All Patient Statuses";
        public static string AllQuestionnaireTypesText => "All Questionnaire Types";
        public static string NoPatientsText => "No Patients";


        public static SelectList GetSitesList(IList<SiteDto> Sites, Guid? SelectedValue, bool IncludeAllSites = true)
        {
            SelectList result;
            if (Sites.Count > 1 && IncludeAllSites)
            {
                if (!Sites.Any(s => s.Name == "All Sites"))
                {
                    Sites.Insert(0, new SiteDto
                    {
                        Id = Guid.Empty,
                        Name = "All Sites"
                    });
                }
            }

            var siteSelects =
                Sites.GroupBy(c => c.Id).Select(group => group.First()).OrderBy(s => s.Name)
                    .Select((x, index) => new {Order = x.Id == Guid.Empty ? -1 : index, x.Id, x.Name}).ToList();

            result = new SelectList(siteSelects.OrderBy(s => s.Order).AsEnumerable(), "Id", "Name", SelectedValue);
            return result;
        }

        public static SelectList GetPatientsList(IList<PatientDto> Patients, Guid SelectedValue = new Guid())
        {
            SelectList result;
            if (Patients.Count == 0)
            {
                Patients.Insert(0, new PatientDto
                {
                    Id = new Guid(),
                    PatientNumber = NoPatientsText
                });
            }
            else if (Patients.Count > 1)
            {
                Patients.Insert(0, new PatientDto
                {
                    Id = new Guid(),
                    PatientNumber = AllPatientsText
                });
            }

            result = new SelectList(Patients, "Id", "PatientNumber", SelectedValue);
            return result;
        }
    }
}