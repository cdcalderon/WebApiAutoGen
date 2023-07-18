using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Dynamic;
using System.Threading.Tasks;
using YPrime.Data.Study.Models;

namespace YPrime.BusinessLayer.UnitTests.Repositories.DataSyncRepositoryTests
{
    [TestClass]
    public class DataSyncRepositoryHandleNewPatientTests : DataSyncRepositoryTestBase
    {
        [TestMethod]
        public async Task DataSyncRepository_HandleNewPatient_IfPatient_CallsAuthService()
        {
            // Arrange
            var id = Guid.NewGuid();
            dynamic entity = new Patient
            {
                Id = id,
                Pin = Pin
            };

            // Act
            await Repository.HandleNewPatientAsync(entity);

            // Assert
            AuthService.Verify(a => a.CreateSubjectAsync(id, Pin), Times.Once);
            Assert.AreEqual("AuthUserId", entity.AuthUserId);
        }

        [TestMethod]
        public async Task DataSyncRepository_HandleNewPatient_IfNotPatient_DoesNotCallAuthService()
        {
            // Arrange
            var id = Guid.NewGuid();
            dynamic entity = new DiaryEntry
            {
                Id = id
            };

            // Act
            await Repository.HandleNewPatientAsync(entity);

            // Assert
            AuthService.Verify(a => a.CreateSubjectAsync(id, Pin), Times.Never);
        }
    }
}
