using Hangfire;
using Hangfire.Common;
using System;
using System.Web.Mvc;
using YPrime.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.StudyPortal.Attributes;

namespace YPrime.StudyPortal.Controllers
{
    public class HangfireJobsController : BaseController
    {
        private readonly IRecurringJobManager _recurringJobManager;

        public HangfireJobsController(ISessionService sessionService, IRecurringJobManager recurringJobManager) : base(sessionService)
        {
            if (recurringJobManager == null) throw new ArgumentNullException(nameof(IRecurringJobManager));

            _recurringJobManager = recurringJobManager;
        }

        [FunctionAuthorization("CanScheduleHangfireJobs", "Can schedule Hangfire jobs")]
        public RedirectToRouteResult ScheduleAllJobs()
        {
            // this is just an example of how to schedule jobs using the IRecurringJobManager and to prove jobs gets scehduled
            _recurringJobManager.AddOrUpdate(nameof(IScheduledJobRepository.TestJob), Job.FromExpression<IScheduledJobRepository>(s => s.TestJob()), Cron.Daily());

            return RedirectToRoute("hangfire");
        }
    }
}
