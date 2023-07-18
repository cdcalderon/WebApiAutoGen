using System;
using System.Collections.Generic;
using System.Linq;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.BusinessLayer.Filters
{
    public class CorrectionFilter : BaseFilter<Correction>
    {
        public CorrectionFilter()
        {
        }

        public override void CreateSiteFilter(List<Site> sites)
        {
            var siteIds = sites.Select(x => x.Id).ToList();
            _siteFilter = (x => siteIds.Contains((Guid)x.SiteId));
        }
    }
}
