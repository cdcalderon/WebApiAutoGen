using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study.Constants;
using YPrime.Data.Study.Models;
using YPrime.StudyPortal.Models;

namespace YPrime.UnitTests.YPrime.StudyPortal.Tests.Controllers.PatientControllerTests
{
    [TestClass]
    public class PatientControllerEditPatientInformationTests : PatientControllerTestBase
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

            var patientStatusList = new List<PatientStatusModel>
            {
                new PatientStatusModel
                {
                    Id = 1,
                    IsActive = true,
                    IsRemoved = false,
                    Name = "Test Status1"
                },
                new PatientStatusModel
                {
                    Id = 2,
                    IsActive = false,
                    IsRemoved = false,
                    Name = "Test Status2"
                },
                new PatientStatusModel
                {
                    Id = 3,
                    IsActive = false,
                    IsRemoved = true,
                    Name = "Test Status3"
                }
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
                    null))
                .ReturnsAsync(_testPatientForEdit);

            DeviceRepository
                .Setup(r => r.GetDeviceIdForPatient(It.Is<Guid>(id => id == _testPatient.Id)))
                .Returns(_testDeviceId);

            PatientStatusService.Setup(p => p.GetAll(It.IsAny<Guid?>())).ReturnsAsync(patientStatusList);
        }

        [TestMethod]
        public async Task EditPatientInformation_CanShowPartialViewTrueTest()
        {
            var result = await Controller.EditPatientInformation(_testPatient.Id,1);

            var castResult = result as PartialViewResult;

            Assert.IsNotNull(castResult);
            Assert.IsNotNull(castResult.ViewBag);
            Assert.IsTrue(castResult.ViewBag.Updated);
            Assert.IsTrue(castResult.ViewBag.ReloadPage);
            Assert.IsTrue(castResult.Model != null);
            Assert.IsTrue(castResult.Model is PatientForEdit);
        }
    }
}
