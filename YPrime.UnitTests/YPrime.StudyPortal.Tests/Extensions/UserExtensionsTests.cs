using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study.Constants;
using YPrime.eCOA.DTOLibrary;
using YPrime.StudyPortal.Extensions;

namespace YPrime.UnitTests.YPrime.StudyPortal.Tests.Extensions
{
    [TestClass]
    public class UserExtensionsTests
    {
        [TestMethod]
        public void CanViewCaregiverTab_TrueTest()
        {
            var testUser = new StudyUserDto
            {
                Roles = new List<StudyRoleModel>
                {
                    new StudyRoleModel
                    {
                        SystemActions = new List<SystemActionModel>
                        {
                            new SystemActionModel
                            {
                                Name = nameof(SystemActionTypes.CanViewCareGiverDetails)
                            }
                        }
                    }
                }
            };

            var result = testUser.CanViewCaregiverTab();

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CanViewCaregiverTab_FalseTest()
        {
            var testUser = new StudyUserDto
            {
                Roles = new List<StudyRoleModel>
                {
                    new StudyRoleModel
                    {
                        SystemActions = new List<SystemActionModel>
                        {
                            new SystemActionModel
                            {
                                Name = nameof(SystemActionTypes.CanViewPatientDetails)
                            }
                        }
                    }
                }
            };

            var result = testUser.CanViewCaregiverTab();

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void CanViewCaregiverTab_NullRolesTest()
        {
            var testUser = new StudyUserDto
            {
                Roles = null
            };

            var result = testUser.CanViewCaregiverTab();

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void CanViewCaregiverTab_NullUserTest()
        {
            StudyUserDto testUser = null;

            var result = testUser.CanViewCaregiverTab();

            Assert.IsFalse(result);
        }
    }
}
