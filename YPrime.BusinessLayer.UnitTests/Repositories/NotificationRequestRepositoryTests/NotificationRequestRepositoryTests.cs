using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using YPrime.BusinessLayer.Repositories;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Data.Study;
using YPrime.Data.Study.Models.Models;

namespace YPrime.BusinessLayer.UnitTests.Repositories.NotificationRequestRepositoryTests
{
    [TestClass]
    public class NotificationRequestRepositoryTests
    {
        private Mock<INotificationScheduleService> _mockNotificationScheduleService;
        private Mock<IStudyDbContext> _mockStudyContext;
        private Guid _testPatientId;

        private NotificationRequest _passedInNotificationRequest;

        [TestInitialize]
        public void TestInitialize()
        {
            _testPatientId = Guid.NewGuid();
            _passedInNotificationRequest = null;

            _mockStudyContext = new Mock<IStudyDbContext>();

            var requestDbSet = new FakeDbSet<NotificationRequest>(new List<NotificationRequest>());

            requestDbSet
                .Setup(e => e.Add(It.IsAny<NotificationRequest>()))
                .Callback((NotificationRequest request) => { _passedInNotificationRequest = request; });

            _mockStudyContext
                .Setup(db => db.NotificationRequests)
                .Returns(requestDbSet.Object);

            _mockNotificationScheduleService = new Mock<INotificationScheduleService>();
        }

        [TestMethod]
        public async Task ProcessCancelationRequest_TrueTest()
        {
            var testResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);

            _mockNotificationScheduleService.
                Setup(s => s.CancelScheduleAsync(It.Is<Guid>(id => id == _testPatientId)))
                .ReturnsAsync(testResponseMessage);

            var repository = GetRepository();

            var result = await repository.ProcessCancelationRequest(_testPatientId);

            Assert.IsTrue(result);

            Assert.IsNotNull(_passedInNotificationRequest);
            Assert.AreEqual(_testPatientId, _passedInNotificationRequest.PatientId);
            Assert.AreEqual((int)HttpStatusCode.OK, _passedInNotificationRequest.ReponseCode);
        }

        [TestMethod]
        public async Task ProcessCancelationRequest_FalseTest()
        {
            var testResponseMessage = new HttpResponseMessage(HttpStatusCode.Unauthorized);

            _mockNotificationScheduleService.
                Setup(s => s.CancelScheduleAsync(It.Is<Guid>(id => id == _testPatientId)))
                .ReturnsAsync(testResponseMessage);

            var repository = GetRepository();

            var result = await repository.ProcessCancelationRequest(_testPatientId);

            Assert.IsFalse(result);

            Assert.IsNotNull(_passedInNotificationRequest);
            Assert.AreEqual(_testPatientId, _passedInNotificationRequest.PatientId);
            Assert.AreEqual((int)HttpStatusCode.Unauthorized, _passedInNotificationRequest.ReponseCode);
        }

        private NotificationRequestRepository GetRepository()
        {
            var repo = new NotificationRequestRepository(
                _mockStudyContext.Object,
                _mockNotificationScheduleService.Object);

            return repo;
        }
    }
}
