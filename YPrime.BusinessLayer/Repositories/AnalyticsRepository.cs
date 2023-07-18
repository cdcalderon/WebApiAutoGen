using System;
using System.Collections.Generic;
using System.Linq;
using YPrime.BusinessLayer.BaseClasses;
using YPrime.BusinessLayer.Exceptions;
using YPrime.BusinessLayer.Interfaces;
using YPrime.BusinessLayer.Session;
using YPrime.Data.Study;
using YPrime.Data.Study.Models;

namespace YPrime.BusinessLayer.Repositories
{
    public class AnalyticsRepository : BaseRepository, IAnalyticsRepository
    {
        public AnalyticsRepository(IStudyDbContext db)
            :base(db)
        {
        }

        public Guid AddAnalyticsReference(AnalyticsReference analyticsReference)
        {
            var entity = _db.AnalyticsReferences.SingleOrDefault(a => a.InternalName == analyticsReference.InternalName);

            if (entity != null)
            {
                throw new DuplicateAnalyticsException();
            }

            entity = _db.AnalyticsReferences.Add(analyticsReference);
            _db.SaveChanges(YPrimeSession.Instance?.CurrentUser?.Id.ToString());

            return entity.Id;
        }

        public List<AnalyticsReference> GetAllAnalyticsReferences()
        {
            return _db.AnalyticsReferences.OrderBy(o => o.DisplayName).ToList();
        }

        public bool RemoveReportByInitialName(string internalName)
        {
            int result = 0;
            var entity =_db.AnalyticsReferences.FirstOrDefault(x => x.InternalName == internalName);

            if (entity != null)
            {
                var studyRole = _db.AnalyticsReferenceStudyRoles.FirstOrDefault(x => x.AnalyticsReferenceId == entity.Id);

                if (studyRole != null)
                {
                    _db.AnalyticsReferenceStudyRoles.Remove(studyRole);
                }

               _db.AnalyticsReferences.Remove(entity);
               result = _db.SaveChanges(YPrimeSession.Instance?.CurrentUser?.Id.ToString());
            }

            return (result > 0);
        }

        /// <summary>
        ///  Updates a report internal and/or display name
        /// </summary>
        /// <param name="internalName">Internal name of the report to be updated</param>
        /// <param name="updatedInternalName">Updated report internal name, will be ignored if empty or null</param>
        /// <param name="updatedDisplayName">Updated report display name, will be ignored if empty or null</param>
        /// <returns>Updated entity</returns>
        public AnalyticsReference UpdateReportName(string internalName, string updatedInternalName, string updatedDisplayName)
        {
            var entity = _db.AnalyticsReferences.FirstOrDefault(x => x.InternalName == internalName);

            if (entity != null)
            {
                if(!string.IsNullOrEmpty(updatedInternalName))
                {
                    entity.InternalName = updatedInternalName;
                }
                if (!string.IsNullOrEmpty(updatedDisplayName))
                {
                    entity.DisplayName = updatedDisplayName;
                }
                _db.SaveChanges(YPrimeSession.Instance?.CurrentUser?.Id.ToString());
            }

            return entity;
        }
    }
}
