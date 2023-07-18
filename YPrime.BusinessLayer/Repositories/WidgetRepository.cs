using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using YPrime.BusinessLayer.BaseClasses;
using YPrime.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Data.Study;
using YPrime.Data.Study.Models;

namespace YPrime.BusinessLayer.Repositories
{
    public class WidgetRepository : BaseRepository, IWidgetRepository
    {
        public WidgetRepository(
            IStudyDbContext db)
            : base(db)
        {
        }

        public async Task<List<Widget>> GetDashboardWidgets(
            string cultureCode, 
            Guid roleId, 
            Guid userId)
        {
            //Init
            var widgets = new List<Widget>();

            /* Get user widgets
            widgets = _db.StudyUserWidgets
                .Where(suw => suw.StudyUserId == userId)
                .ToList()
                .Select(s => new Widget(s.Widget, s.WidgetPosition)).ToList();
            */

            if (widgets.Count == 0)
            {
                //Fall back to role widgets
                widgets = _db.StudyRoleWidgets
                    .Where(srw => srw.StudyRoleId == roleId)
                    .ToList()
                    .Select(s => new Widget(s.Widget, s.WidgetPosition)).ToList();
            }

            //Translate
            await TranslateWidgets(widgets, cultureCode, userId);
            return widgets;
        }

        public async Task<List<Widget>> GetWidgets(string cultureCode, Guid roleId, Guid userId)
        {   
            var userSystemActions = 
                _db.SystemActionStudyRoles.Where(x => x.StudyRoleId == roleId)
                .Select(sa => sa.SystemActionId);

            var widgets = await _db.Widgets
                .Where(w => w.WidgetSystemActions.Count == 0 || w.WidgetSystemActions.Any(wsa => userSystemActions.Contains(wsa.SystemActionId)))
                .ToListAsync();

            await TranslateWidgets(widgets, cultureCode, userId);

            return widgets.OrderBy(w => w.TitleTextDisplay).ToList();
        }

        public async Task<List<Widget>> GetWidgets(string cultureCode)
        {
            List<Widget> result = null;
            result = await _db.Widgets.Include("WidgetType").OrderBy(w => w.WidgetPosition).ToListAsync();
            await TranslateWidgets(result, cultureCode);
            return result;
        }

        private async Task TranslateWidgets(List<Widget> widgets, string cultureCode, Guid? userId = null)
        {
            foreach (var w in widgets)
            {
                w.TitleTextDisplay = w.TranslationTitleText;
                w.DescriptionTextDisplay = w.TranslationDescriptionText;
                w.ButtonTextDisplay = w.TranslationButtonText;

                foreach (var wc in w.WidgetCounts)
                {
                    wc.Text = wc.TranslationText;
                }

                foreach (var wl in w.WidgetLinks)
                {
                    wl.Text = wl.TranslationText;
                }

                w.Counts = CreateWidgetCounts(w);
                w.WidgetSystemActions.ToList().ForEach(swa =>
                {
                    swa.SystemAction = _db.SystemActions.Single(sa => sa.Id == swa.SystemActionId);
                });
            }
        }

        private List<KeyValuePair<int, string>> CreateWidgetCounts(Widget w)
        {
            var result = new List<KeyValuePair<int, string>>();

            w.WidgetCounts.ToList().ForEach(wc =>
            {
                result.Add(new KeyValuePair<int, string>(GetWidgetCount(wc), wc.Text));
            });

            return result;
        }

        private int GetWidgetCount(WidgetCount widgetCount)
        {
            var result = 0;

            if (!string.IsNullOrWhiteSpace(widgetCount.FunctionName))
            {
                //call the repo method
                Type thisType = this.GetType();
                MethodInfo widgetMethod = thisType.GetMethod(widgetCount.FunctionName);

                object[] parameters = null;

                result = (int)widgetMethod.Invoke(this, parameters);
            }
            else
            {
                //get the table data
                var sql = "select count(*) from [" + widgetCount.TableName + "]";
                result = _db.Database.SqlQuery<int>(sql).First();
            }
            return result;
        }

        public async Task<Widget> GetWidget(Guid id, string cultureCode, Guid? userId = null)
        {
            var widget = _db.Widgets.Single(w => w.Id == id);
            await TranslateWidgets(new List<Widget>() { widget }, cultureCode, userId);
            return widget;
        }

        public void SaveStudyUserWidgets(Guid userId, List<StudyUserWidget> studyUserWidgets)
        {
            using (DbContextTransaction transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    var user = _db.StudyUsers.Single(u => u.Id == userId);
                    _db.StudyUserWidgets.RemoveRange(user.StudyUserWidgets);
                    user.StudyUserWidgets = studyUserWidgets;
                    _db.SaveChanges(null);
                    transaction.Commit();
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateConcurrencyException)
                {
                    transaction.Rollback();
                }
            }
        }


        public void SaveStudyRoleWidgets(Guid roleId, List<StudyRoleWidget> studyRoleWidgets)
        {
            var oldWidgets = _db.StudyRoleWidgets.Where(r => r.Id == roleId);
            _db.StudyRoleWidgets.RemoveRange(oldWidgets);
            _db.StudyRoleWidgets.AddRange(studyRoleWidgets);
            _db.SaveChanges(null);
        }
    }
}
