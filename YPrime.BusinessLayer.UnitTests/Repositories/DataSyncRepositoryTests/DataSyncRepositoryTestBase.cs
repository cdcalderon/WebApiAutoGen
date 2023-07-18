using Moq;
using System;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using YPrime.Auth.Data.Context;
using YPrime.BusinessLayer.DataSync.Factories;
using YPrime.BusinessLayer.Interfaces;
using YPrime.BusinessLayer.Repositories;
using YPrime.BusinessLayer.UnitTests.TestObjects;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Models;
using YPrime.Data.Study;
using YPrime.Data.Study.Models;

namespace YPrime.BusinessLayer.UnitTests.Repositories.DataSyncRepositoryTests
{
    public abstract class DataSyncRepositoryTestBase : RepositoryTestBase
    {
        protected readonly string Pin = "1234";

        protected readonly Mock<IStudyDbContext> Context;        
        protected readonly DataSyncRepository Repository;
        protected Mock<ISoftwareReleaseRepository> SoftwareReleaseRepository;
        protected Mock<IDeviceRepository> DeviceRepository;
        protected Mock<IPatientStatusService> PatientStatusService;
        protected Mock<ISqlBuilderFactory> SqlBuilderFactory;
        protected Mock<IAuthService> AuthService;
        protected Mock<IPatientRepository> PatientRepository;

        protected DataSyncRepositoryTestBase()
        {
            Context = new Mock<IStudyDbContext>();            
            SoftwareReleaseRepository = new Mock<ISoftwareReleaseRepository>();
            DeviceRepository = new Mock<IDeviceRepository>();
            PatientStatusService = new Mock<IPatientStatusService>();
            SqlBuilderFactory = new Mock<ISqlBuilderFactory>();
            AuthService = new Mock<IAuthService>();
            PatientRepository= new Mock<IPatientRepository>();

            SqlBuilderFactory
                .Setup(f => f.Build(It.IsAny<Guid?>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<bool>()));

            AuthService.Setup(x => x.CreateSubjectAsync(It.IsAny<Guid>(), It.IsAny<string>())).Returns(Task.FromResult(new AuthUserSignupResponse { UserId = "AuthUserId" }));
            
            PatientRepository
                .Setup(pr => pr.DecryptPin(It.IsAny<Patient>())).Returns((Patient p) => p.Pin);

            Repository = new DataSyncRepository(
                Context.Object,                 
                SoftwareReleaseRepository.Object, 
                DeviceRepository.Object,
                PatientStatusService.Object,
                SqlBuilderFactory.Object,
                AuthService.Object,
                PatientRepository.Object);
        }
    }
}
