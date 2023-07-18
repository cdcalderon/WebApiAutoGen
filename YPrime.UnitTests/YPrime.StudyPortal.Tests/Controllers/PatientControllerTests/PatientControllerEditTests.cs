using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study.Constants;
using YPrime.Data.Study.Models;
using YPrime.StudyPortal.Models;

namespace YPrime.UnitTests.YPrime.StudyPortal.Tests.Controllers.PatientControllerTests
{
    [TestClass]
    public class PatientControllerEditTests : PatientControllerTestBase
    {
        private readonly Guid _testDeviceId = Guid.NewGuid();

        private Patient _testPatient;
        private PatientForEdit _testPatientForEdit;

        protected override void OnInitialize()
        {
            base.OnInitialize();

            _testPatient = new Patient
            {
                Id = Guid.NewGuid()
            };

            _testPatientForEdit = new PatientForEdit
            {
                Id = _testPatient.Id,
                PatientHasDevice = true
            };

            PatientRepository
                .Setup(r => r.GetPatientForEditAsync(It.Is<Guid>(id => id == _testPatient.Id)))
                .ReturnsAsync(_testPatient);

            PatientForEditAdapter
                .Setup(a => a.Adapt(
                    It.IsAny<Patient>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.Is<Guid?>(did => did == _testDeviceId),
                    It.Is<Guid?>(q => q == CurrentUserId)))
                .ReturnsAsync(_testPatientForEdit);

            DeviceRepository
                .Setup(r => r.GetDeviceIdForPatient(It.Is<Guid>(id => id == _testPatient.Id)))
                .Returns(_testDeviceId);

            _yprimeSession.StudySettingValues = new Dictionary<string, string>
            {
                { "BringYourOwnDeviceAvailable", "false" }
            };
        }

        [TestMethod]
        public async Task Edit_CanShowCaregiversTrueTest()
        {
            _yprimeSession.CurrentUser.Roles.Clear();

            _yprimeSession.CurrentUser.Roles.Add(new StudyRoleModel
            {
                SystemActions = new List<SystemActionModel>
                { 
                    new SystemActionModel
                    {
                        Name = nameof(SystemActionTypes.CanViewCareGiverDetails)
                    }
                }
            });

            _yprimeSession.StudySettingValues.Add("CaregiversEnabled", "1");

            var result = await Controller.Edit(_testPatient.Id);

            var castResult = result as ViewResult;

            Assert.IsNotNull(castResult);
            Assert.IsNotNull(castResult.ViewBag);
            Assert.IsTrue(castResult.ViewBag.ShowCaregiverTab);
        }

        [TestMethod]
        public async Task Edit_CanShowCaregiversFalsePermissionTest()
        {
            _yprimeSession.CurrentUser.Roles.Clear();

            _yprimeSession.CurrentUser.Roles.Add(new StudyRoleModel
            {
                SystemActions = new List<SystemActionModel>
                {
                    new SystemActionModel
                    {
                        Name = nameof(SystemActionTypes.CanViewPatientDetails)
                    }
                }
            });

            _yprimeSession.StudySettingValues.Add("CaregiversEnabled", "1");

            var result = await Controller.Edit(_testPatient.Id);

            var castResult = result as ViewResult;

            Assert.IsNotNull(castResult);
            Assert.IsNotNull(castResult.ViewBag);
            Assert.IsFalse(castResult.ViewBag.ShowCaregiverTab);
        }

        [TestMethod]
        public async Task Edit_CanShowCaregiversFalse_ResetPinPermissionTest()
        {
            _yprimeSession.CurrentUser.Roles.Clear();

            _yprimeSession.CurrentUser.Roles.Add(new StudyRoleModel
            {
                SystemActions = new List<SystemActionModel>
                {
                    new SystemActionModel
                    {
                        Name = nameof(SystemActionTypes.CanResetCareGiverPin)
                    }
                }
            });

            _yprimeSession.StudySettingValues.Add("CaregiversEnabled", "1");

            var result = await Controller.Edit(_testPatient.Id);

            var castResult = result as ViewResult;

            Assert.IsNotNull(castResult);
            Assert.IsNotNull(castResult.ViewBag);
            Assert.IsFalse(castResult.ViewBag.ShowCaregiverTab);
        }

        [TestMethod]
        public async Task Edit_CanShowCaregiversFalseStudySettingTest()
        {
            _yprimeSession.CurrentUser.Roles.Clear();

            _yprimeSession.CurrentUser.Roles.Add(new StudyRoleModel
            {
                SystemActions = new List<SystemActionModel>
                {
                    new SystemActionModel
                    {
                        Name = nameof(SystemActionTypes.CanResetCareGiverPin)
                    }
                }
            });

            _yprimeSession.StudySettingValues.Add("CaregiversEnabled", "0");

            var result = await Controller.Edit(_testPatient.Id);

            var castResult = result as ViewResult;

            Assert.IsNotNull(castResult);
            Assert.IsNotNull(castResult.ViewBag);
            Assert.IsFalse(castResult.ViewBag.ShowCaregiverTab);
        }
    }
}
