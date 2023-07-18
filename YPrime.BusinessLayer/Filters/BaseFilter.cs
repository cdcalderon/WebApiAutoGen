using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading;
using System.Web.Mvc;
using YPrime.BusinessLayer.Interfaces;
using YPrime.Data.Study.Models;

namespace YPrime.BusinessLayer.Filters
{
    public abstract class BaseFilter<T>
    {
        protected readonly IList<Expression<Func<T, bool>>> _filters = new List<Expression<Func<T, bool>>>();
        protected Expression<Func<T, bool>> _siteFilter;

        private readonly IUserRepository _userRepo = DependencyResolver.Current.GetService<IUserRepository>();

        public abstract void CreateSiteFilter(List<Site> sites);
      
        private Guid? GetUserIdFromHttpContext()
        {           
            var principal = (ClaimsPrincipal)Thread.CurrentPrincipal;
            var userId = principal?.Claims?.FirstOrDefault(c => c.Type == "https://auth.eclinicalcloud.net/ypAuthUserId")?.Value ?? null;
            // Inline is not supported.
            Guid userIdAsGuid;
            if (!string.IsNullOrWhiteSpace(userId)
                && Guid.TryParse(userId, out userIdAsGuid))
            {
                return userIdAsGuid;
            }
            return null;
        }

        private List<Site> GetUserSites()
        {
            List<Site> allowedSites = null;
            var userId = GetUserIdFromHttpContext();
            if (userId != null)
            {
                allowedSites = _userRepo.GetValidSitesForUser((Guid)userId);             
            }
            return allowedSites;
        }

        private IQueryable<T> ExecuteWithSiteFitler(IQueryable<T> records)
        {       
            List<Site> validSites = GetUserSites();
            if (validSites != null && validSites.Any())
            {
                CreateSiteFilter(validSites);
            }

            // add Siter filter to all patient record retrival 
            var siteFilter = _siteFilter != null ? _siteFilter : null;
            if (siteFilter != null && !_filters.Contains(siteFilter))
            {
                _filters.Add(siteFilter);
            }

            // do base execute
            records = ExecuteBase(records);

            // remove site filter from filters and clear siteFilter
            if (siteFilter != null)
            {
                _filters.Remove(siteFilter);
                _siteFilter = null;
            }

            return records;
        }

        private IQueryable<T> ExecuteBase(IQueryable<T> records)
        {
            foreach (var filter in _filters)
            {
                records = records.Where(filter);
            }
            
            return records;
        }

        public IQueryable<T> Execute(IQueryable<T> records)
        {         
             return ExecuteWithSiteFitler(records);                   
        }
    }
}
