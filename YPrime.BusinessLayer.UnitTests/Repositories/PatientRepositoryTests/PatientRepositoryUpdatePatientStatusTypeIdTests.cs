using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YPrime.Data.Study.Models;

namespace YPrime.BusinessLayer.UnitTests.Repositories.PatientRepositoryTests
{

    [TestClass]
    public class PatientRepositoryUpdatePatientStatusTypeIdTests : PatientRepositoryTestBase
    {
        private const int _inactivePatientStatusTypeId = 4;
        private const int _activePatientStatusTypeId = 2;
        private Patient _testPatient;

        [TestInitialize]
        public void TestInitialize()
        {
            _testPatient = new Patient
            {
                Id = Guid.NewGuid(),
                PatientNumber = "S-1001-123456",
                IsHandheldTrainingComplete = true,
                IsTabletTrainingComplete = true,
                IsTempPin = false,
                EnrolledDate = DateTime.Now.AddDays(-10).Date,
                SiteId = BaseSite.Id,
                Site = BaseSite,
            };

            BasePatients.Add(_testPatient);
        }

        [TestMethod]
        public async Task UpdatePatientStatusTypeIdTest()
        {
            _testPatient.PatientStatusTypeId = _activePatientStatusTypeId;

            MockNotificationRequestRepositoy
                .Setup(s => s.ProcessChangePatientStatusRequest(It.IsAny<Guid>(), It.IsAny<bool>()))
                .ReturnsAsync(true);

            await Repository.UpdatePatientStatusTypeId(_testPatient.Id, _inactivePatientStatusTypeId);

            MockContext.Verify(c => c.SaveChanges(It.IsAny<string>()), Times.Once);
            MockNotificationRequestRepositoy.Verify(s => s.ProcessChangePatientStatusRequest(It.IsAny<Guid>(), It.IsAny<bool>()));
            MockContext.ResetCalls();
        }

        [TestMethod]
        public async Task UpdateNotificationScheduleForPatientTest()
        {
            MockNotificationRequestRepositoy
                .Setup(s => s.ProcessChangePatientStatusRequest(It.Is<Guid>(pid => pid == _testPatient.Id), It.IsAny<bool>()))
                .ReturnsAsync(true);

            await Repository.UpdateNotificationScheduleForPatient(_testPatient.Id, _inactivePatientStatusTypeId, _activePatientStatusTypeId);

            MockNotificationRequestRepositoy
                .Verify(s => s.ProcessChangePatientStatusRequest(It.Is<Guid>(pid => pid == _testPatient.Id), It.IsAny<bool>()));

            MockContext.ResetCalls();
        }

        /// <summary>
        /// /////////
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task UpdateNotificationScheduleForReactivatedPatientTest()
        {
            MockNotificationRequestRepositoy
                .Setup(s => s.ProcessChangePatientStatusRequest(It.Is<Guid>(pid => pid == _testPatient.Id), It.IsAny<bool>()))
                .ReturnsAsync(true);

            await Repository.UpdateNotificationScheduleForPatient(_testPatient.Id, _activePatientStatusTypeId, _inactivePatientStatusTypeId);

            MockNotificationRequestRepositoy
                .Verify(s => s.ProcessChangePatientStatusRequest(It.Is<Guid>(pid => pid == _testPatient.Id), It.IsAny<bool>()));

            MockContext.ResetCalls();
        }
    }
}
