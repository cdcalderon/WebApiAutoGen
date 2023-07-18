using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YPrime.Data.Study.Models;
using YPrime.Data.Study.Models.Interfaces;

namespace YPrime.BusinessLayer.UnitTests.Repositories.DataSyncRepositoryTests
{
    [TestClass]
    public class DataSyncRepositoryHandleUpdatedPatientPinTests : DataSyncRepositoryTestBase
    {
        [TestMethod]
        public async Task DataSyncRepository_HandleUpdatedPatientPin_IfPatient_PinsDiffer_CallsAuthService()
        {
            // Arrange
            var id = Guid.NewGuid();
            dynamic entity = new Patient
            {
                Id = id,
                Pin = Pin,
                AuthUserId = "AuthUserId"
            };

            var serverPatient = new Patient
            {
                Id = id,
                Pin = "4321"
            };

            // Act
            await Repository.HandleUpdatedPatientPinAsync(entity, serverPatient);

            // Assert
            AuthService.Verify(a => a.ChangePasswordAsync("AuthUserId", Pin), Times.Once);
        }

        [TestMethod]
        public async Task DataSyncRepository_HandleUpdatedPatientPin_IfPatient_PinsSame_DoesNotCallAuthService()
        {
            // Arrange
            var id = Guid.NewGuid();
            dynamic entity = new Patient
            {
                Id = id,
                Pin = Pin,
                AuthUserId = "AuthUserId"
            };

            var serverPatient = new Patient
            {
                Id = id,
                Pin = Pin
            };

            // Act
            await Repository.HandleUpdatedPatientPinAsync(entity, serverPatient);

            // Assert
            AuthService.Verify(a => a.ChangePasswordAsync("AuthUserId", Pin), Times.Never);
        }

        [TestMethod]
        public async Task DataSyncRepository_HandleUpdatedPatientPin_IfNotPatient_DoesNotCallAuthService()
        {
            // Arrange
            var id = Guid.NewGuid();
            dynamic entity = new DiaryEntry
            {
                Id = id
            };

            // Act
            await Repository.HandleUpdatedPatientPinAsync(entity, entity);

            // Assert
            AuthService.Verify(a => a.ChangePasswordAsync("AuthUserId", Pin), Times.Never);
        }
    }
}
