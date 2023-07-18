using Hangfire;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Web.Mvc;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.eCOA.DTOLibrary;
using YPrime.StudyPortal.Controllers;
using YPrime.UnitTests.YPrime.PatientPortal.Tests.Controllers;

namespace YPrime.UnitTests.YPrime.StudyPortal.Tests.Controllers.ScheduledJobControllerTests
{
    public abstract class HangfireJobsControllerTestBase : BaseControllerTest
    {
        protected HangfireJobsController Controller;
        protected Mock<ISessionService> _sessionService;
        protected Mock<IRecurringJobManager> _recurringJobManager;

        [TestInitialize]
        public virtual void TestInitialize()
        {
            _yprimeSession.CurrentUser = new StudyUserDto
            {
                Roles = new List<Core.BusinessLayer.Models.StudyRoleModel>
                {
                    new Core.BusinessLayer.Models.StudyRoleModel
                    {
                        ShortName = "YP"
                    }
                }
            };

            base.Initialize();

            _sessionService = new Mock<ISessionService>();
            _recurringJobManager = new Mock<IRecurringJobManager>();

            Controller = new HangfireJobsController(_sessionService.Object, _recurringJobManager.Object)
            {
                ControllerContext = (new Mock<ControllerContext>()).Object
            };
        }
    }
}
