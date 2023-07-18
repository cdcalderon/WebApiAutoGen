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
    public class PatientRepositoryResetPatientPinTests : PatientRepositoryTestBase
    {
        private Patient _testPatient;

        [TestInitialize]
        public void Initialize()
        {
            _testPatient = new Patient
            {
                Id = Guid.NewGuid(),
                Pin = "1234",
                SyncVersion = 0,
                IsTempPin = false,
                AccessFailedCount = 0,
                LockoutEnabled = false,
                AuthUserId = "AuthUserId"
            };

            BasePatients.Add(_testPatient);
        }

        [TestMethod]
        [Ignore] // This logic has been removed. Leaving the test here in case logic is re-added later.
        public async Task PatientRepository_ResetPatientPin_CallsAuthService()
        {
            // Arrange
            var newPin = "4321";

            // Act
            await Repository.ResetPatientPin(_testPatient.Id, newPin);

            // Assert
            MockAuthService.Verify(a => a.ChangePasswordAsync(_testPatient.AuthUserId, newPin), Times.Once);
        }
    }
}
