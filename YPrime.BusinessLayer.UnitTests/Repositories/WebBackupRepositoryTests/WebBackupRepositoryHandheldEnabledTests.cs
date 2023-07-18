using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YPrime.Core.BusinessLayer.Models;

namespace YPrime.BusinessLayer.UnitTests.Repositories.WebBackupRepositoryTests
{
    [TestClass]
    public class WebBackupRepositoryHandheldEnabledTests : WebBackupRepositoryTestBase
    {
        private StudySettingModel _data1, _data2, _data3;
        private List<StudySettingModel> _dataSet;

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize();
            _data1 = new StudySettingModel
            {
                Properties = new StudySettingProperties
                {
                    Key = "WebBackupHandheldEnabled",
                },
                Value = "3"
            };
            _data2 = new StudySettingModel
            {
                Properties = new StudySettingProperties
                {
                    Key = "WebBackupHandheldEnabled",
                },
                Value = "ASD"
            };
            _data3 = new StudySettingModel
            {
                Properties = new StudySettingProperties
                {
                    Key = "WebBackupHandheldEnabled",
                },
                Value = null
            };
        }

        private void SetupContext(IEnumerable<StudySettingModel> items)
        {
            if (_dataSet != null)
            {
                _dataSet.Clear();
            }
            else
            {
                _dataSet = new List<StudySettingModel>();
            }
            _dataSet.AddRange(items);
        }

        [TestMethod]
        public async Task WhenWebBackupHandheldEnabledIsEnteredWith_CurrectNumVal()
        {
            SetupContext(new[] { _data1 });

            MockStudySettingService
                .Setup(x => x.GetStringValue(It.Is<string>(s => s == "WebBackupHandheldEnabled"), It.IsAny<Guid?>()))
                .ReturnsAsync(_data1.Value);

            var repo = GetRepository();
                
            var numDays = await repo.GetWebBackupHandheldValue();
            var expectedNumDays = int.Parse(_data1.Value);
            Assert.AreEqual(expectedNumDays, numDays);
        }

        [TestMethod]
        public async Task WhenWebBackupHandheldEnabledIsEnteredWith_StringVal()
        {
            SetupContext(new[] { _data2 });

            MockStudySettingService
             .Setup(x => x.GetStringValue(It.Is<string>(s => s == "WebBackupHandheldEnabled"), It.IsAny<Guid?>()))
             .ReturnsAsync(_data2.Value);

            var repo = GetRepository();

            var numDays = await repo.GetWebBackupHandheldValue();
            Assert.AreEqual(-1, numDays);
        }

        [TestMethod]
        public async Task WhenWebBackupHandheldEnabledIsEnteredWith_NullVal()
        {
            SetupContext(new[] { _data3 });

            MockStudySettingService
               .Setup(x => x.GetStringValue(It.Is<string>(s => s == "WebBackupHandheldEnabled"), It.IsAny<Guid?>()))
               .ReturnsAsync(_data3.Value);

            var repo = GetRepository();

            var numDays = await repo.GetWebBackupHandheldValue();
            Assert.AreEqual(-1, numDays);
        }
    }
}