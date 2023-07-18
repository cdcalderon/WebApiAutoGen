using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YPrime.BusinessLayer.UnitTests;
using YPrime.eCOA.DTOLibrary.WebBackup;

namespace YPrime.UnitTests.YPrime.StudyPortal.Tests.Controllers.WebBackupControllerTests
{
    [TestClass]
    public class WebBackupControllerSendWebBackupEmailTests : WebBackupControllerTestBase
    {
        [TestMethod]
        public async Task WhenCalled_WillReturnJsonResultType()
        {
            var _Dto = new WebBackupEmailModel
            {
                Id = Guid.NewGuid(),
                EmailContentId = Guid.NewGuid(),
                PatientEmail = "test@yprime.com",
                EmailContent = "test content",
                Subject = "test subject",
                WebBackupJwtModel = new WebBackupJwtModel()
            };
            var result = await Controller.SendWebBackupEmail(_Dto);

            YAssert.IsType<JsonResult>(result);
        }

        [TestMethod]
        public void WhenCalled_WillReturnModelStateIsFalse()
        {
            Controller.ModelState.AddModelError("test", "test");
            var _Dto = new WebBackupEmailModel
            {
                Id = Guid.NewGuid(),
                EmailContentId = Guid.NewGuid(),
                PatientEmail = "test@yprime.com",
                EmailContent = "test content",
                Subject = "test subject",
                WebBackupJwtModel = new WebBackupJwtModel()
            };
            Controller.SendWebBackupEmail(_Dto);

            Assert.IsFalse(Controller.ModelState.IsValid);
        }
    }
}