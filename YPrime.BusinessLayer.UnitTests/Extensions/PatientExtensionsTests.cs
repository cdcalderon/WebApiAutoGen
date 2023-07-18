using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using YPrime.BusinessLayer.Extensions;
using YPrime.Config.Enums;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study.Models;

namespace YPrime.BusinessLayer.UnitTests.Extensions
{
    [TestClass]
    public class PatientExtensionsTests
    {

        [TestMethod]
        public void PatientExtensionsGetPatientStatusTypeTest()
        {
            var statusTypes = new List<PatientStatusModel>
            {
                new PatientStatusModel
                {
                    Id = 2, 
                    Name = "Enrolled"
                },
                new PatientStatusModel
                {
                    Id = 3,
                    Name = "Removed"
                }
            };
            var expectedStatusType = statusTypes.First();
            var patient = new Patient
            {
                PatientStatusTypeId = expectedStatusType.Id
            };

            var result = patient.GetPatientStatusType(statusTypes);

            Assert.AreEqual(expectedStatusType, result);
        }
    }
}
