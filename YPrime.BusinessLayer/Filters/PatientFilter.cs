using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using YPrime.Data.Study.Models;
using YPrime.Config.Enums;
using YPrime.Core.BusinessLayer.Models;
using YPrime.BusinessLayer.Interfaces.Filters;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.BusinessLayer.Filters
{
    public class PatientFilter : BaseFilter<Patient>, IPatientFilter
    {   
        public PatientFilter(List<PatientStatusModel> removedStatuses, bool dataCorrection = false)
        {
            if (removedStatuses == null)
            {
                removedStatuses = new List<PatientStatusModel>();
            }

            FilterRemoved(removedStatuses, dataCorrection);
        }

        public IPatientFilter ById(Guid id)
        {
            _filters.Add(p => p.Id == id);
            return this;
        }

        public IPatientFilter ByPatientStatusTypeId(int? patientStatusTypeId)
        {
            _filters.Add(p => p.PatientStatusTypeId == patientStatusTypeId);
            return this;
        }

        public IPatientFilter BySiteId(Guid? siteId)
        {
            _filters.Add(p => p.SiteId == siteId);
            return this;
        }

        public override void CreateSiteFilter(List<Site> sites)
        {
            var siteIds = sites.Select(x => x.Id).ToList();
            _siteFilter = (x => siteIds.Contains(x.SiteId));
        }

        public IPatientFilter ByPatientNumber(string patientNumber)
        {
            _filters.Add(p => string.Equals(p.PatientNumber, patientNumber, StringComparison.CurrentCultureIgnoreCase));
            return this;
        }

        public IPatientFilter ByCountryId(Guid? countryId)
        {
            _filters.Add(p => p.Site.CountryId == countryId);
            return this;
        }

        private void FilterRemoved(List<PatientStatusModel> removedStatuses, bool dataCorrection)
        {
            if(!dataCorrection)
            {       
                foreach (var status in removedStatuses)
                {
                    _filters.Add(p => p.PatientStatusTypeId != status.Id);
                }
            }
        }
    }
}