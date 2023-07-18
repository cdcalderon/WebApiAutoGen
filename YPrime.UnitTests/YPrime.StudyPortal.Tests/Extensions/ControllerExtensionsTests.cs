using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq;
using YPrime.Config.Enums;
using YPrime.BusinessLayer.Interfaces;
using YPrime.StudyPortal.Controllers;
using YPrime.StudyPortal.Extensions;
using YPrime.UnitTests.YPrime.PatientPortal.Tests.Controllers;
using YPrime.Core.BusinessLayer.Models;
using System.Collections.Generic;

namespace YPrime.UnitTests.YPrime.StudyPortal.Tests.Extensions
{
    [TestClass]
    public class ControllerExtensionsTests : BaseControllerTest
    {
        [TestMethod]
        public void GetPatientStatusTypesSelectListExcludeRemovedTest()
        {
            var mockConfirmationRepository = new Mock<IConfirmationRepository>();
            var mockRoleRepository = new Mock<IRoleRepository>();

            var controller = new ConfirmationController(
                mockConfirmationRepository.Object,
                mockRoleRepository.Object,
                MockSessionService.Object);

            var statusTypes = new List<PatientStatusModel>
            {
                new PatientStatusModel
                {
                    Id = 1,
                    Name = "Screened"
                },
                new PatientStatusModel
                {
                    Id = 2,
                    Name = "Removed",
                    IsRemoved = true
                }
            };

            var result = controller.GetPatientStatusTypesSelectList(
                statusTypes.First().Id, 
                statusTypes,
                true);

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(1, result.Where(r => r.Selected).Count());

            var selectedValue = result.First(r => r.Selected);
            Assert.AreEqual(statusTypes.First().Id, int.Parse(selectedValue.Value));
            Assert.AreEqual(statusTypes.First().Name, selectedValue.Text);
        }

        [TestMethod]
        public void GetPatientStatusTypesSelectListIncludeRemovedTest()
        {
            var mockConfirmationRepository = new Mock<IConfirmationRepository>();
            var mockRoleRepository = new Mock<IRoleRepository>();

            var controller = new ConfirmationController(
                mockConfirmationRepository.Object,
                mockRoleRepository.Object,
                MockSessionService.Object);

            var statusTypes = new List<PatientStatusModel>
            {
                new PatientStatusModel
                {
                    Id = 1,
                    Name = "Enrolled"
                },
                new PatientStatusModel
                {
                    Id = 2,
                    Name = "Removed", 
                    IsRemoved = true
                }
            };

            var result = controller.GetPatientStatusTypesSelectList(
                statusTypes.First().Id,
                statusTypes,
                false);

            Assert.AreEqual(2, result.Count());
            Assert.AreEqual(1, result.Where(r => r.Selected).Count());

            var selectedValue = result.First(r => r.Selected);
            Assert.AreEqual(statusTypes.First().Id, int.Parse(selectedValue.Value));
            Assert.AreEqual(statusTypes.First().Name, selectedValue.Text);
        }
    }
}
