using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using YPrime.Core.BusinessLayer.Interfaces;

namespace YPrime.BusinessLayer.UnitTests.Repositories.SiteRepositoryTests
{
    [TestClass]
    public class SiteRepositoryWebBackupDateTests : SiteRepositoryTestBase
    {
        [TestMethod]
        public async Task CheckDateCalculationReflectsBackupDays()
        {
            Mock<IStudySettingService> MockStudySettingsService = new Mock<IStudySettingService>();
            int NumDays = await MockStudySettingsService.Object.GetIntValue("WebBackupTabletEnabled");

            DateTime? newDate = await repository.CalculateWebBackupExpireDate("Eastern Standard Time");
            if (NumDays < 1)
            {
                Assert.AreEqual(newDate, null);
            }
            else
            {
                TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                DateTime localToday = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);

                string ExpectedDate = localToday.AddDays(NumDays).ToShortDateString();
                string ReturnedDate = newDate?.ToShortDateString();
                Assert.AreEqual(ExpectedDate, ReturnedDate);
            }
        }
    }
}