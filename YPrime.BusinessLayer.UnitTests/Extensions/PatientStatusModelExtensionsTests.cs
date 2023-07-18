using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Core.BusinessLayer.Extensions;

namespace YPrime.BusinessLayer.UnitTests.Extensions
{
    [TestClass]
    public class PatientStatusModelExtensionsTests
    {
        [TestMethod]
        public void WhenPatientStatusIsActiveEqualsFalseShouldReturnTrue()
        {
            var patientStatus = new PatientStatusModel
            {
                IsActive = false
            };

            var result = patientStatus.IsDisabled();

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void WhenPatientStatusIsRemovedEqualsTrueShouldReturnTrue()
        {
            var patientStatus = new PatientStatusModel
            {
                IsRemoved = true
            };

            var result = patientStatus.IsDisabled();

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void WhenPatientStatusHasAnEnabledStatusShouldReturnFalse()
        {
            // An Enabled status could be IsActive = true or IsRemoved = false;
            var patientStatus = new PatientStatusModel
            {
                IsActive = true,
                IsRemoved = false
            };

            var result = patientStatus.IsDisabled();

            Assert.IsFalse(result);
        }
    }
}
