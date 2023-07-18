using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using YPrime.Data.Study.Configurations.Entity;
using YPrime.Data.Study.Models;
using YPrime.Data.Study.Models.Interfaces;
using YPrime.Data.Study.Models.Models;
using YPrime.Data.Study.Proxies;
using YPrime.eCOA.DTOLibrary.Utils;

namespace YPrime.Data.Study
{
    public class StudyDbContext : DbContext, IStudyDbContext
    {
        private const string _systemUser = "SYSTEM";
        private const string _apiAuditSource = "A";
        private const string _modelAssemblyName = "YPrime.Data.Study.Models";

        public StudyDbContext() : base("StudyContext")
        {
            // To enforce model errors, keep this commented:
            Database.SetInitializer<StudyDbContext>(null);
        }

        public StudyDbContext(string connString) : base(connString) { }

        public StudyDbContext(string connString, IPrincipal currentUser) : base(connString)
        {
            Database.SetInitializer<StudyDbContext>(null);
        }

        public virtual DbSet<ExportStatus> ExportStatuses { get; set; }

        public Guid? YpStudyUserIdFromHeader { get; set; }

        public Guid? CorrectionId { get; set; }

        public List<object> Configurations { get; private set; } = new List<object>();

        public virtual DbSet<SiteLanguage> SiteLanguages { get; set; }
        public virtual DbSet<Answer> Answers { get; set; }
        public virtual DbSet<AnalyticsReference> AnalyticsReferences { get; set; }
        public virtual DbSet<AnalyticsReferenceStudyRole> AnalyticsReferenceStudyRoles { get; set; }
        public virtual DbSet<CareGiver> CareGivers { get; set; }
        public virtual DbSet<CorrectionApprovalData> CorrectionApprovalDatas { get; set; }
        public virtual DbSet<CorrectionApprovalDataAdditional> CorrectionApprovalDataAdditionals { get; set; }
        public virtual DbSet<Correction> Corrections { get; set; }
        public virtual DbSet<CorrectionStatus> CorrectionStatuses { get; set; }
        public virtual DbSet<CorrectionHistory> CorrectionHistories { get; set; }
        public virtual DbSet<CorrectionAction> CorrectionActions { get; set; }
        public virtual DbSet<CorrectionWorkflow> CorrectionWorkflows { get; set; }
        public virtual DbSet<CorrectionDiscussion> CorrectionDiscussions { get; set; }
        public virtual DbSet<DataApproval> DataApprovals { get; set; }
        public virtual DbSet<DCFRequest> DCFRequest { get; set; }
        public virtual DbSet<DiaryEntry> DiaryEntries { get; set; }
        public virtual DbSet<Device> Devices { get; set; }
        public virtual DbSet<DeviceData> DeviceDatas { get; set; }
        public virtual DbSet<DeviceSession> DeviceSessions { get; set; }
        public virtual DbSet<EmailRecipient> EmailRecipients { get; set; }
        public virtual DbSet<EmailContent> EmailContents { get; set; }
        public virtual DbSet<EmailSent> EmailSents { get; set; }
        public virtual DbSet<Export> Exports { get; set; }
        public virtual DbSet<Patient> Patients { get; set; }
        public virtual DbSet<PatientAttribute> PatientAttributes { get; set; }
        public virtual DbSet<PatientVisit> PatientVisits { get; set; }
        public virtual DbSet<ReferenceMaterial> ReferenceMaterials { get; set; }
        public virtual DbSet<ReportStudyRole> ReportStudyRoles { get; set; }
        public virtual DbSet<EmailContentStudyRole> EmailContentStudyRoles { get; set; }
        public virtual DbSet<SecurityQuestion> SecurityQuestions { get; set; }
        public virtual DbSet<Site> Sites { get; set; }
        public virtual DbSet<SiteActiveHistory> SiteActiveHistory { get; set; }
        public virtual DbSet<StudyRoleUpdate> StudyRoleUpdates { get; set; }
        public virtual DbSet<StudyRoleWidget> StudyRoleWidgets { get; set; }
        public virtual DbSet<StudyUser> StudyUsers { get; set; }
        public virtual DbSet<StudyUserRole> StudyUserRoles { get; set; }
        public virtual DbSet<StudyUserWidget> StudyUserWidgets { get; set; }
        public virtual DbSet<SyncLog> SyncLogs { get; set; }
        public virtual DbSet<SoftwareVersion> SoftwareVersions { get; set; }
        public virtual DbSet<SoftwareRelease> SoftwareReleases { get; set; }
        public virtual DbSet<SoftwareReleaseCountry> SoftwareReleaseCountry { get; set; }
        public virtual DbSet<SoftwareReleaseDeviceType> SoftwareReleaseDeviceTypes { get; set; }
        public virtual DbSet<SystemAction> SystemActions { get; set; }
        public virtual DbSet<SystemActionStudyRole> SystemActionStudyRoles { get; set; }
        public virtual DbSet<SystemSetting> SystemSettings { get; set; }
        public virtual DbSet<VisitComplianceReportQuestionnaireType> VisitComplianceReportQuestionnaireTypes { get; set; }
        public virtual DbSet<Widget> Widgets { get; set; }
        public virtual DbSet<WidgetCount> WidgetCounts { get; set; }
        public virtual DbSet<WidgetLink> WidgetLinks { get; set; }
        public virtual DbSet<WidgetSystemAction> WidgetSystemActions { get; set; }
        public virtual DbSet<MissedVisitReason> MissedVisitReasons { get; set; }
        public virtual DbSet<AdditionalTablesToSync> AdditionalTablesToSync { get; set; }
        public virtual DbSet<ScreenReportDialog> ScreenReportDialog { get; set; }
        public virtual DbSet<InputFieldTypeResult> InputFieldTypeResults { get; set; }
        public virtual DbSet<QuestionInputFieldTypeResult> QuestionInputFieldTypeResults { get; set; }
        public virtual DbSet<NotificationRequest> NotificationRequests { get; set; }

        /// <summary>
        /// Updates Audit columns in all New/Modified entities before context flushes changes to Database
        /// </summary>
        private void UpdateAuditInfoForPendingEntities(string userId)
        {
            foreach (var entity in ChangeTracker.Entries()
               .Where(e => e.State == EntityState.Added
                || e.State == EntityState.Modified))
            {
                var auditModel = entity.Entity as IAuditModel;
                if (auditModel != null)
                {
                    auditModel.AuditUserID = userId;
                }
            }
        }

        public int SaveChanges(string userId)
        {
            userId = YpStudyUserIdFromHeader.HasValue
                ? YpStudyUserIdFromHeader.Value.ToString()
                : userId ?? _systemUser;

            UpdateSyncVersionForSyncObjects();
            UpdateAuditInfoForPendingEntities(userId);

            //Store username in userId variable for report display purposes
            Guid id;
            if (Guid.TryParse(userId, out id))
            {
                var user = StudyUsers.FirstOrDefault(x => x.Id == id);
                userId = user?.UserName ?? userId;
            }
            
            string sql = BuildAuditSQL(userId);

            var result = base.SaveChanges();

            if (sql != string.Empty)
            {
                Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction ,sql);
            }

            return result;
        }

        public async Task<int> SaveChangesAsync(string username)
        {
            username = YpStudyUserIdFromHeader.HasValue
                ? YpStudyUserIdFromHeader.Value.ToString()
                : username ?? _systemUser;

            UpdateSyncVersionForSyncObjects();
            UpdateAuditInfoForPendingEntities(username);

            var sql = BuildAuditSQL(username);
            var result = await base.SaveChangesAsync();

            if (sql != string.Empty)
            {
                await Database.ExecuteSqlCommandAsync(TransactionalBehavior.DoNotEnsureTransaction, sql);
            }

            return result;
        }

        public void SaveDeviceAudits(List<dynamic> deviceAuditEntries)
        {
            var auditSql = BuildDeviceAuditSQL(deviceAuditEntries);

            if (!string.IsNullOrWhiteSpace(auditSql))
            {
                Database.ExecuteSqlCommand(auditSql);
            }
        }

        public List<T> ExecuteSqlToList<T>(string sql, params object[] parameters)
        {
            return Database.SqlQuery<T>(sql, parameters).ToList();
        }

        public IEnumerable<dynamic> CollectionFromSqlStoredProcedure(
            string storedProcedure,
            Dictionary<string, object> parameters)
        {
            using (var cmd = Database.Connection.CreateCommand())
            {
                cmd.CommandText = storedProcedure;
                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();

                cmd.CommandType = CommandType.StoredProcedure;

                foreach (var param in parameters)
                {
                    var sqlParameter = new SqlParameter($"@{param.Key}", param.Value);
                    cmd.Parameters.Add(sqlParameter);
                }

                using (var dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        var dataRow = GetDataRow(dataReader);
                        yield return dataRow;
                    }
                }
            }
        }

        public void LoadPropertyFromDb<TEntity, TProperty>(TEntity entity,
            Expression<Func<TEntity, TProperty>> navigationProperty)
            where TEntity : class
            where TProperty : class
        {
            Entry(entity).Reference(navigationProperty).Load();
        }

        public void DetatchEntity<TEntity>(TEntity entity)
            where TEntity : class
        {
            Entry(entity).State = EntityState.Detached;
        }

        // wrappers to make BeginTransaction mockable
        public IDbContextTransactionProxy BeginTransaction()
        {
            return new DbContextTransactionProxy(this);
        }

        public IDbContextTransactionProxy BeginTransaction(IsolationLevel isolationLevel)
        {
            return new DbContextTransactionProxy(this, isolationLevel);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var convention = new AttributeToColumnAnnotationConvention<DefaultValueAttribute, string>(
                "SqlDefaultValue",
                (p, attributes) => attributes.SingleOrDefault().Value.ToString());
            modelBuilder.Conventions.Add(convention);

            //Set configurations -- need to add entity from model on instantiate to preserver scaffolding in MVC apps referencing the model
            Configurations = new List<object>
            {
                new ScreenReportDialogConfiguration(modelBuilder.Entity<ScreenReportDialog>()),
                new AnswerConfiguration(modelBuilder.Entity<Answer>()),
                new DeviceConfiguration(modelBuilder.Entity<Device>()),
                new DiaryEntryConfiguration(modelBuilder.Entity<DiaryEntry>()),
                new EmailContentConfiguration(modelBuilder.Entity<EmailContent>()),
                new ExportConfiguration(modelBuilder.Entity<Export>()),
                new ExportStatusConfiguration(modelBuilder.Entity<ExportStatus>()),
                new PatientConfiguration(modelBuilder.Entity<Patient>()),
                new PatientAttributeConfiguration(modelBuilder.Entity<PatientAttribute>()),
                new PatientVisitConfiguration(modelBuilder.Entity<PatientVisit>()),
                new ReferenceMaterialConfiguration(modelBuilder.Entity<ReferenceMaterial>()),
                new EmailContentStudyRoleConfiguration(modelBuilder.Entity<EmailContentStudyRole>()),
                new SecurityQuestionConfiguration(modelBuilder.Entity<SecurityQuestion>()),
                new SiteConfiguration(modelBuilder.Entity<Site>()),
                new SyncLogConfiguration(modelBuilder.Entity<SyncLog>()),
                new SystemActionStudyRoleConfiguration(modelBuilder.Entity<SystemActionStudyRole>()),
                new CorrectionActionConfiguration(modelBuilder.Entity<CorrectionAction>()),
                new WidgetConfiguration(modelBuilder.Entity<Widget>()),
                new SoftwareVersionConfiguration(modelBuilder.Entity<SoftwareVersion>()),
                new SoftwareReleaseConfiguration(modelBuilder.Entity<SoftwareRelease>())
            };

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
        }

        private string BuildAuditSQL(string username)
        {
            var sql = new StringBuilder();
            foreach (var entity in ChangeTracker.Entries().Where(e =>
                e.State == EntityState.Added
                    || e.State == EntityState.Modified
                    || e.State == EntityState.Deleted))
            {
                var insert = GetInsertSQL(entity.Entity,
                    entity.State == EntityState.Added ? "A" : entity.State == EntityState.Deleted ? "D" : "U",
                    username);
                sql.AppendLine(insert);
            }

            return sql.ToString();
        }

        private string BuildDeviceAuditSQL(List<dynamic> deviceAuditEntries)
        {
            var sqlBuilder = new StringBuilder();

            foreach (var auditRecords in deviceAuditEntries)
            {
                var originalEntityTableName = auditRecords.TableName;
                var typeName = $"{_modelAssemblyName}.{originalEntityTableName}, {_modelAssemblyName}";
                var entityType = Type.GetType(typeName, true, true);

                var rows = auditRecords.Rows.ToObject<List<dynamic>>();

                foreach (var dynamicEntity in rows)
                {
                    DateTimeOffset modifiedDate;
                    if (!DateTimeOffset.TryParse(dynamicEntity.ModifiedDate?.ToString(), out modifiedDate))
                    {
                        modifiedDate = DateTimeOffset.Now;
                    }

                    Guid? auditDeviceId = null;
                    Guid parsedAuditDeviceId;
                    if (Guid.TryParse(dynamicEntity.AuditDeviceId?.ToString(), out parsedAuditDeviceId))
                    {
                        auditDeviceId = parsedAuditDeviceId;
                    }

                    var entity = dynamicEntity.ToObject(entityType);

                    var insertStatement = BuildAuditInsertStatement(
                        entity,
                        entityType,
                        auditRecords.TableName?.ToString(),
                        dynamicEntity.AuditAction?.ToString(),
                        dynamicEntity.AuditSource?.ToString(),
                        dynamicEntity.ModifiedBy?.ToString(),
                        modifiedDate,
                        null,
                        auditDeviceId);

                    sqlBuilder.AppendLine(insertStatement);
                }
            }

            return sqlBuilder.ToString();
        }

        private void UpdateSyncVersionForSyncObjects()
        {
            foreach (var entity in ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added
                    || e.State == EntityState.Modified))
            {
                var syncObject = entity.Entity as IDataSyncObject;
                if (syncObject != null)
                {
                    syncObject.SyncVersion++;
                }
            }
        }

        private string GetInsertSQL(object obj, string auditAction, string username)
        {
            var originalEntityType = ObjectContext.GetObjectType(obj.GetType());

            var tableName = originalEntityType.Name;

            var attr = Attribute.GetCustomAttribute(originalEntityType, typeof(TableAttribute), true);
            if (attr != null)
            {
                tableName = ((TableAttribute)attr).Name.Replace("dbo.", string.Empty);
            }

            if (auditAction == "D")
            {
                //delete removes all the values from the entity object
                //we need to retrieve them before the DELETE is actually run.
                var databaseValues = Entry(obj).GetDatabaseValues();

                foreach (var propertyInfo in originalEntityType.GetProperties().ToList())
                {
                    var t = propertyInfo.PropertyType;
                    var mapped = propertyInfo.GetCustomAttributes(typeof(NotMappedAttribute), false).Length == 0;
                    var isPrimitiveType = t.IsPrimitive
                        || t.IsValueType
                        || (t == typeof(string));
                    if (isPrimitiveType
                        && mapped
                        && !propertyInfo.PropertyType.IsArray
                        //&& !propertyInfo.PropertyType.IsGenericType
                        )
                    {
                        var propertyValue = databaseValues[propertyInfo.Name];
                        obj.SetPropertyValue(propertyInfo.Name, propertyValue);
                    }
                }
            }

            var insertStatement = BuildAuditInsertStatement(
                obj,
                originalEntityType,
                tableName,
                auditAction,
                _apiAuditSource,
                username,
                DateTimeOffset.Now,
                CorrectionId);

            return insertStatement;
        }

        private string BuildAuditInsertStatement(
            object obj,
            Type entityType,
            string tableName,
            string auditAction,
            string auditSource,
            string modifiedBy,
            DateTimeOffset modifiedDate,
            Guid? auditCorrectionId,
            Guid? auditDeviceId = null)
        {
            var columns = new StringBuilder();
            var values = new StringBuilder();

            foreach (var prop in entityType.GetProperties())
            {             
                var targetPropertyType = obj.GetPropertyType(prop.Name);

                //check for nullable
                if (Nullable.GetUnderlyingType(targetPropertyType) != null)
                {
                    targetPropertyType = Nullable.GetUnderlyingType(targetPropertyType);
                }

                var isList = targetPropertyType.IsGenericType
                             && (targetPropertyType.GetGenericTypeDefinition() == typeof(IList<>)
                                 || targetPropertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                                 || targetPropertyType.GetGenericTypeDefinition() == typeof(ICollection<>));

                var mapped = prop.GetCustomAttributes(typeof(NotMappedAttribute), false).Length == 0;

                if ((targetPropertyType.Name == "String"
                    || !targetPropertyType.IsClass && !isList) && mapped)
                {
                    var valueIsNull = prop.GetValue(obj, null) == null;
                    switch (targetPropertyType.Name)
                    {
                        case "Guid":
                            columns.Append($"[{prop.Name}],");
                            values.Append($"{(valueIsNull ? "null" : "'" + prop.GetValue(obj) + "'")},");
                            break;

                        case "String":
                            if (!valueIsNull)
                            {
                                columns.Append($"[{prop.Name}],");
                                values.Append($"'{DoubleDelimiter((string)prop.GetValue(obj))}',");
                            }
                            break;

                        case "Nullable`1":
                            if (!valueIsNull)
                            {
                                columns.Append($"[{prop.Name}],");
                                values.Append($"'{prop.GetValue(obj)}',");
                            }
                            break;

                        default:
                            if (!valueIsNull)
                            {
                                columns.Append($"[{prop.Name}],");
                                values.Append($"'{prop.GetValue(obj)}',");
                            }
                            break;
                    }
                }
            }

            columns.Append("AuditAction,");
            values.Append($"'{auditAction}',");

            columns.Append("ModifiedBy,");
            values.Append($"'{modifiedBy}',");

            columns.Append("ModifiedDate,");
            values.Append($"'{modifiedDate}',");

            columns.Append("AuditCorrectionId,");
            values.Append((auditCorrectionId.HasValue ? $"'{auditCorrectionId}'," : "null,"));

            columns.Append("AuditSource,");
            values.Append($"'{auditSource}',");

            columns.Append("AuditDeviceId");
            values.Append($"{(auditDeviceId.HasValue ? $"'{auditDeviceId.Value}'" : "null")}");

            var sql = new StringBuilder();
            sql.AppendLine($"Insert Into ypaudit.{tableName} ");
            sql.AppendLine("(");
            sql.Append(columns);
            sql.AppendLine(")");
            sql.AppendLine("values");
            sql.AppendLine("(");
            sql.Append(values);
            sql.AppendLine(");");
            return sql.ToString();
        }

        private string DoubleDelimiter(string value)
        {
            return (value.Replace("'", "''"));
        }

        private static dynamic GetDataRow(DbDataReader dataReader)
        {
            var dataRow = new ExpandoObject() as IDictionary<string, object>;
            for (var fieldCount = 0; fieldCount < dataReader.FieldCount; fieldCount++)
                dataRow.Add(dataReader.GetName(fieldCount), dataReader[fieldCount]);
            return dataRow;
        }
    }
}
