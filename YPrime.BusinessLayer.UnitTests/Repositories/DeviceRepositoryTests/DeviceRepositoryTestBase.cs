using System;
using System.Collections.Generic;
using Moq;
using YPrime.BusinessLayer.Interfaces;
using YPrime.BusinessLayer.Repositories;
using YPrime.Data.Study;
using YPrime.Data.Study.Models;
using YPrime.Core.BusinessLayer.Interfaces;

namespace YPrime.BusinessLayer.UnitTests.Repositories.DeviceRepositoryTests
{
    public abstract class DeviceRepositoryTestBase
    {
        protected const string HandheldDeviceTypeName = "Phone";
        protected const string TabletDeviceTypeName = "Tablet";

        protected readonly Mock<IStudyDbContext> Context;
        protected Mock<ISoftwareReleaseRepository> _softwareReleaseRepository;
        protected Mock<IConfigurationVersionService> _configurationVersionService;
        protected readonly DeviceRepository Repository;

        protected DeviceRepositoryTestBase()
        {
            Context = new Mock<IStudyDbContext>();
            _softwareReleaseRepository = new Mock<ISoftwareReleaseRepository>();
            _configurationVersionService = new Mock<IConfigurationVersionService>();
            Repository = new DeviceRepository(Context.Object, null, _softwareReleaseRepository.Object, _configurationVersionService.Object);
        }

        protected void SetupContext(IEnumerable<Device> devices)
        {
            var dbSet = new FakeDbSet<Device>(devices);

            Context.Setup(ctx => ctx.Devices)
                .Returns(dbSet.Object);
        }

        protected Device CreateTestDevice(Guid siteId, Guid deviceTypeId)
        {
            var testDevice = new Device
            {
                Id = Guid.NewGuid(),
                DeviceTypeId = deviceTypeId,
                PatientId = Guid.NewGuid(),
                SiteId = siteId
            };

            return testDevice;
        }
    }
}