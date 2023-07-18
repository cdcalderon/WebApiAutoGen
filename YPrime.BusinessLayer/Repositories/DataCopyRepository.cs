using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using YPrime.BusinessLayer.BaseClasses;
using YPrime.BusinessLayer.Interfaces;
using YPrime.BusinessLayer.Session;
using YPrime.Data.Study;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.BusinessLayer.Repositories
{
    public class DataCopyRepository : BaseRepository, IDataCopyRepository
    {
        public DataCopyRepository(
            IStudyDbContext db)
            : base(db)
        { }

        public async Task<StudyPortalConfigDataDto> GetStudyPortalConfigData()
        {
            var result = new StudyPortalConfigDataDto
            {
                EmailContentStudyRoles = await _db.EmailContentStudyRoles.ToListAsync(),
                ReportStudyRoles = await _db.ReportStudyRoles.ToListAsync(),
                StudyRoleUpdates = await _db.StudyRoleUpdates.ToListAsync(),
                StudyRoleWidgets = await _db.StudyRoleWidgets.ToListAsync(),
                SystemActionStudyRoles = await _db.SystemActionStudyRoles.ToListAsync(),
                AnalyticsReferences = await _db.AnalyticsReferences.ToListAsync(),
                AnalyticsReferenceStudyRoles = await _db.AnalyticsReferenceStudyRoles.ToListAsync(),
            };

            var sanitizedResult = SanitizeStudyPortalConfigData(result);

            return sanitizedResult;
        }

        public async Task UpdateStudyPortalConfigData(StudyPortalConfigDataDto studyConfig)
        {
            using (var transaction = _db.BeginTransaction())
            {
                try
                {
                    await ProcessStudyConfigData(
                            _db.EmailContentStudyRoles,
                            studyConfig.EmailContentStudyRoles.ToArray());

                    await ProcessStudyConfigData(
                            _db.ReportStudyRoles,
                            studyConfig.ReportStudyRoles.ToArray());

                    await ProcessStudyConfigData(
                            _db.StudyRoleUpdates,
                            studyConfig.StudyRoleUpdates.ToArray());

                    await ProcessStudyConfigData(
                            _db.StudyRoleWidgets,
                            studyConfig.StudyRoleWidgets.ToArray());

                    await ProcessStudyConfigData(
                            _db.SystemActionStudyRoles,
                            studyConfig.SystemActionStudyRoles.ToArray());

                    await ProcessStudyConfigData(
                          _db.AnalyticsReferences,
                          studyConfig.AnalyticsReferences.ToArray());

                    await ProcessStudyConfigData(
                            _db.AnalyticsReferenceStudyRoles,
                            studyConfig.AnalyticsReferenceStudyRoles.ToArray());

                    await _db.SaveChangesAsync(YPrimeSession.Instance?.CurrentUser?.Id.ToString());

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        private StudyPortalConfigDataDto SanitizeStudyPortalConfigData(StudyPortalConfigDataDto model)
        {
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            };

            var serializedModel = JsonConvert.SerializeObject(model, settings);

            var result = JsonConvert.DeserializeObject<StudyPortalConfigDataDto>(serializedModel, settings);

            RemoveEmailContentStudyRoleChildData(result);
            RemoveStudyRoleWidgetChildData(result);
            RemoveSystemActionStudyRoleChildData(result);
            RemoveAnalyticsReferenceStudyRoleChildData(result);
             
            return result;
        }

        private void RemoveEmailContentStudyRoleChildData(StudyPortalConfigDataDto configData)
        {
            foreach (var entity in configData.EmailContentStudyRoles)
            {
                entity.EmailContent = null;
            }
        }

        private void RemoveStudyRoleWidgetChildData(StudyPortalConfigDataDto configData)
        {
            foreach (var entity in configData.StudyRoleWidgets)
            {
                entity.Widget = null;
            }
        }

        private void RemoveSystemActionStudyRoleChildData(StudyPortalConfigDataDto configData)
        {
            foreach (var entity in configData.SystemActionStudyRoles)
            {
                entity.SystemAction = null;
            }
        }

        private void RemoveAnalyticsReferenceStudyRoleChildData(StudyPortalConfigDataDto configData)
        {
            foreach(var entity in configData.AnalyticsReferenceStudyRoles)
            {
                entity.AnalyticsReference = null;
            }
        }

        private async Task ProcessStudyConfigData<T>(DbSet<T> dbSet, T[] entities)
            where T : class
        {
            dbSet.AddOrUpdate(entities);

            await RemoveResults(dbSet, entities);
        }

        private async Task RemoveResults<T>(
            DbSet<T> dbSet,
            T[] updatedEntities)
            where T : class
        {
            if (updatedEntities == null || !updatedEntities.Any())
            {
                return;
            }

            var entityType = typeof(T);
            var entityTypeName = entityType.Name;

            var dbEntities = await dbSet.ToListAsync();
            var resultsToRemove = new List<T>();

            foreach (var dbEntity in dbEntities)
            {
                var shouldDeleteEntity = false;

                switch (entityTypeName)
                {
                    case nameof(EmailContentStudyRole):
                        shouldDeleteEntity = FindMatchingEmailContentStudyRole(
                            dbEntity as EmailContentStudyRole,
                            updatedEntities as EmailContentStudyRole[]);
                        break;
                    case nameof(ReportStudyRole):
                        shouldDeleteEntity = FindMatchingReportStudyRole(
                            dbEntity as ReportStudyRole,
                            updatedEntities as ReportStudyRole[]);
                        break;
                    case nameof(SystemActionStudyRole):
                        shouldDeleteEntity = FindMatchingSystemActionStudyRole(
                            dbEntity as SystemActionStudyRole,
                            updatedEntities as SystemActionStudyRole[]);
                        break;
                    case nameof(StudyRoleUpdate):
                        shouldDeleteEntity = FindMatchingStudyRoleUpdate(
                            dbEntity as StudyRoleUpdate,
                            updatedEntities as StudyRoleUpdate[]);
                        break;
                    case nameof(StudyRoleWidget):
                        shouldDeleteEntity = FindMatchingStudyRoleWidget(
                            dbEntity as StudyRoleWidget,
                            updatedEntities as StudyRoleWidget[]);
                        break;
                    default:
                        shouldDeleteEntity = false;
                        break;
                }

                if (shouldDeleteEntity)
                {
                    resultsToRemove.Add(dbEntity);
                }
            }

            if (resultsToRemove.Any())
            {
                dbSet.RemoveRange(resultsToRemove);
            }
        }

        private bool FindMatchingEmailContentStudyRole(
            EmailContentStudyRole dbEntity,
            EmailContentStudyRole[] updatedEntities)
        {
            var matchingUpdatedEntity = updatedEntities
                .FirstOrDefault(ue =>
                    ue.EmailContentId == dbEntity.EmailContentId &&
                    ue.StudyRoleId == dbEntity.StudyRoleId);

            return matchingUpdatedEntity == null;
        }

        private bool FindMatchingReportStudyRole(
            ReportStudyRole dbEntity,
            ReportStudyRole[] updatedEntities)
        {
            var matchingUpdatedEntity = updatedEntities
                .FirstOrDefault(ue =>
                    ue.ReportId == dbEntity.ReportId &&
                    ue.StudyRoleId == dbEntity.StudyRoleId);

            return matchingUpdatedEntity == null;
        }

        private bool FindMatchingSystemActionStudyRole(
            SystemActionStudyRole dbEntity,
            SystemActionStudyRole[] updatedEntities)
        {
            var matchingUpdatedEntity = updatedEntities
                .FirstOrDefault(ue =>
                    ue.SystemActionId == dbEntity.SystemActionId &&
                    ue.StudyRoleId == dbEntity.StudyRoleId);

            return matchingUpdatedEntity == null;
        }

        private bool FindMatchingStudyRoleUpdate(
            StudyRoleUpdate dbEntity,
            StudyRoleUpdate[] updatedEntities)
        {
            var matchingUpdatedEntity = updatedEntities
                .FirstOrDefault(ue =>
                    ue.StudyRoleId == dbEntity.StudyRoleId &&
                    ue.LastUpdate == dbEntity.LastUpdate);

            return matchingUpdatedEntity == null;
        }

        private bool FindMatchingStudyRoleWidget(
            StudyRoleWidget dbEntity,
            StudyRoleWidget[] updatedEntities)
        {
            var matchingUpdatedEntity = updatedEntities
                .FirstOrDefault(ue =>
                    ue.Id == dbEntity.Id);

            return matchingUpdatedEntity == null;
        }
    }
}
