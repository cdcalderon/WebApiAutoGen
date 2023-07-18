using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using YPrime.BusinessLayer.UnitTests;
using YPrime.eCOA.DTOLibrary.WebBackup;

namespace YPrime.UnitTests.YPrime.StudyPortal.Tests.Controllers.WebBackupControllerTests
{
    [TestClass]
    public class WebBackupControllerSendWebBackupEmailWithVisitActivationTests : WebBackupControllerTestBase
    {
        [TestMethod]
        public async Task WhenCalled_WillReturnJsonResultType()
        {
            var _Dto = new WebBackupEmailWithVisitActivationModel
            {
                Id = Guid.NewGuid(),
                EmailContentId = Guid.NewGuid(),
                PatientEmail = "test@yprime.com",
                EmailContent = "test content",
                Subject = "test subject",
                WebBackupJwtModel = new WebBackupJwtModel(),
                VisitId = Guid.NewGuid(),
                PatientId = Guid.NewGuid()
            };
            var result = await Controller.SendWebBackupEmailWithVisitActivation(_Dto);

            YAssert.IsType<JsonResult>(result);
        }

        [TestMethod]
        public void WhenCalled_WillReturnModelStateIsFalse()
        {
            Controller.ModelState.AddModelError("test", "test");
            var _Dto = new WebBackupEmailWithVisitActivationModel
            {
                Id = Guid.NewGuid(),
                EmailContentId = Guid.NewGuid(),
                PatientEmail = "test@yprime.com",
                EmailContent = "test content",
                Subject = "test subject",
                WebBackupJwtModel = new WebBackupJwtModel(),
                VisitId = Guid.NewGuid(),
                PatientId = Guid.NewGuid()
            };
            Controller.SendWebBackupEmailWithVisitActivation(_Dto);

            Assert.IsFalse(Controller.ModelState.IsValid);
        }
    }
}
