using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using YPrime.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.eCOA.DTOLibrary;
using YPrime.StudyPortal.Controllers;
using YPrime.UnitTests.YPrime.PatientPortal.Tests.Controllers;

namespace YPrime.StudyPortal.Tests.Controllers.SoftwareVersionControllerTests
{
    public abstract class SoftwareVersionControllerTestBase : BaseControllerTest
    {
        protected SoftwareVersionController Controller;
        protected Mock<ISoftwareVersionRepository> Repository;
        protected Mock<ISessionService> MockSessionService;

        [TestInitialize]
        public void TestInitialize()
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
            Repository = new Mock<ISoftwareVersionRepository>();
            MockSessionService = new Mock<ISessionService>();

            Controller = new SoftwareVersionController(
                Repository.Object,
                MockSessionService.Object);

            OnInitialize();
        }

        protected virtual void OnInitialize()
        {
        }
    }
}