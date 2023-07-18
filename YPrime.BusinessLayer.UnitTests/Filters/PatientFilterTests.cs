using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using YPrime.BusinessLayer.Filters;
using YPrime.Config.Enums;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study.Models;

namespace YPrime.BusinessLayer.UnitTests.Filters
{
    [TestClass]
    public class PatientFilterTests
    {
        private readonly Guid PatientSiteId = new Guid();

        private List<Patient> _testPatients = new List<Patient>();
        private Patient _activePatient;
        private Patient _inactivePatient;
        private Patient _removedPatient;
        private List<PatientStatusModel> _patientStatuses;

        [TestInitialize]
        public void TestInitialize()
        {
            _patientStatuses = new List<PatientStatusModel>
            {
                new PatientStatusModel
                {
                    Id = 2,
                    Name = "Enrolled",
                    IsActive = true, 
                    IsRemoved = false
                },
                new PatientStatusModel
                {
                    Id = 3,
                    Name = "Completed",
                    IsRemoved = false
                },
                new PatientStatusModel
                {
                    Id = 4,
                    Name = "Removed",
                    IsRemoved = true
                }
            };
            _activePatient = new Patient
            {
                Id = Guid.NewGuid(),
                PatientNumber = "1",
                PatientStatusTypeId = _patientStatuses.First(s => s.IsActive).Id,
                SiteId = PatientSiteId,
            };

            _inactivePatient = new Patient
            {
                Id = Guid.NewGuid(),
                PatientNumber = "2",
                PatientStatusTypeId = _patientStatuses.First(s => s.Name == "Completed").Id,
                SiteId = PatientSiteId
            };

            _removedPatient = new Patient
            {
                Id = Guid.NewGuid(),
                PatientNumber = "3",
                PatientStatusTypeId = _patientStatuses.First(s => s.IsRemoved).Id,
                SiteId = PatientSiteId
            };

            _testPatients = new List<Patient>
            {
                _activePatient,
                _inactivePatient,
                _removedPatient
            };
        }

        [TestMethod]
        public void PatientFilterConstructorTest()
        {
            const int expectedCount = 2;

            var filter = new PatientFilter(_patientStatuses.Where(p => p.IsRemoved).ToList());

            var result = filter.Execute(_testPatients.AsQueryable()).ToList();

            Assert.AreEqual(expectedCount, result.Count());
            Assert.IsFalse(result.Any(p => p.Id == _removedPatient.Id));
        }

        [TestMethod]
        public void PatientFilterByIdTest()
        {
            const int expectedCount = 1;

            var filter = new PatientFilter(_patientStatuses.Where(p => p.IsRemoved).ToList());
            filter.ById(_activePatient.Id);

            var result = filter.Execute(_testPatients.AsQueryable()).ToList();

            Assert.AreEqual(expectedCount, result.Count());
            Assert.IsTrue(result[0].Id == _activePatient.Id);
        }

        [TestMethod]
        public void PatientFilterByPatientStatusTypeIdTest()
        {
            const int expectedCount = 1;

            var filter = new PatientFilter(_patientStatuses.Where(p => p.IsRemoved).ToList());
            filter.ByPatientStatusTypeId(_activePatient.PatientStatusTypeId);

            var result = filter.Execute(_testPatients.AsQueryable()).ToList();

            Assert.AreEqual(expectedCount, result.Count());
            Assert.IsTrue(result[0].Id == _activePatient.Id);
        }

        [TestMethod]
        public void PatientFilterBySiteIdTest()
        {
            const int expectedCount = 2;

            var filter = new PatientFilter(_patientStatuses.Where(p => p.IsRemoved).ToList());
            filter.BySiteId(PatientSiteId);

            var result = filter.Execute(_testPatients.AsQueryable()).ToList();

            Assert.AreEqual(expectedCount, result.Count());
            Assert.IsFalse(result.Any(p => p.Id == _removedPatient.Id));
        }

        [TestMethod]
        public void PatientFilterInAllowableSitesTest()
        {
            const int expectedCount = 2;

            var filter = new PatientFilter(_patientStatuses.Where(p => p.IsRemoved).ToList());
      
            var result = filter.Execute(_testPatients.AsQueryable()).ToList();

            Assert.AreEqual(expectedCount, result.Count());
            Assert.IsFalse(result.Any(p => p.Id == _removedPatient.Id));
        }

        [TestMethod]
        public void PatientFilterByPatientNumberTest()
        {
            const int expectedCount = 1;

            var filter = new PatientFilter(_patientStatuses.Where(p => p.IsRemoved).ToList());
            filter.ByPatientNumber(_inactivePatient.PatientNumber);

            var result = filter.Execute(_testPatients.AsQueryable()).ToList();

            Assert.AreEqual(expectedCount, result.Count());
            Assert.AreEqual(result[0].Id, _inactivePatient.Id);
        }
    }
}
