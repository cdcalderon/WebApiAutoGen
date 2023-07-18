using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.BusinessLayer.Filters
{
    public class SiteFilter : BaseFilter<Site>
    {
        public override void CreateSiteFilter(List<Site> sites)
        {
            var siteIds = sites.Select(x => x.Id).ToList();
            _siteFilter = (x => siteIds.Contains(x.Id));
        } 
    }
}
