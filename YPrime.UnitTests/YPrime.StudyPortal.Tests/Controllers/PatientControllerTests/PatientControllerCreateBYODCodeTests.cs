using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using YPrime.eCOA.DTOLibrary;
using YPrime.StudyPortal.Models;

namespace YPrime.UnitTests.YPrime.StudyPortal.Tests.Controllers.PatientControllerTests
{
    [TestClass]
    public class PatientControllerCreateBYODCodeTests : PatientControllerTestBase
    {
        [TestMethod]
        public async Task PatientControllerCreateBYODCodePatientIsBYODTest()
        {
            var result = await Controller.CreateBYODCode(BYODPatientId);
            var data = result.Data as AjaxResult;
            var json = JsonConvert.DeserializeObject<PatientJson>(data.JsonData);

            Assert.IsNotNull(result);
            Assert.IsTrue(data.Success);
            Assert.AreEqual(json.PatientId, BYODPatientId);
        }

        [TestMethod]
        public async Task PatientControllerCreateBYODCodePatientIsProvisionalTest()
        {
            var result = await Controller.CreateBYODCode(PatientId);
            var data = result.Data as AjaxResult;
            var json = JsonConvert.DeserializeObject<PatientJson>(data.JsonData);

            Assert.IsNotNull(result);
            Assert.IsTrue(data.Success);
            Assert.AreEqual(json.PatientId, PatientId);
        }
    }

    partial class PatientJson
    {
        public Guid PatientId { get; set; }
    }
}
