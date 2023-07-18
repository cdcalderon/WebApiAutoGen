using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using YPrime.BusinessLayer.Constants;
using YPrime.BusinessLayer.Interfaces;
using YPrime.BusinessLayer.Services;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.eCOA.DTOLibrary;
using YPrime.StorageService.Services.Interfaces;
using YPrime.StorageService.Services.Models;

namespace YPrime.BusinessLayer.UnitTests.Services.FileServiceTests
{
    public abstract class FileServiceTestBase
    {
        protected const string ImageContentType = "image/png";
        protected const string ImageFileName = "image.png";
        protected const string TestStudyId = "823451f8-d47c-4a9e-904d-4d4115c8957a";
        protected const string ZipContentType = "application/zip";
        protected const string TestConnectionString = "connectionString";

        protected static readonly Guid TestExportId = Guid.NewGuid();
        protected static readonly Guid TestPatientId = Guid.NewGuid();
        protected static readonly Guid TestQuestionnaireId = Guid.NewGuid();
        protected static readonly Guid TestReferenceMaterialId = Guid.NewGuid();
        protected static readonly Guid TestUserId = Guid.NewGuid();
        protected static readonly Guid TestVisitId = Guid.NewGuid();

        protected static readonly string ExportFullPath = FormatAsFilePath(TestStudyId, TestUserId.ToString(), $"{TestExportId}.zip");
        protected static readonly string PatientDataFullPath = FormatAsFilePath(TestStudyId, TestPatientId.ToString(), TestVisitId.ToString(), TestQuestionnaireId.ToString(), ImageFileName);
        protected static readonly string ReferenceMaterialFullPath = FormatAsFilePath(TestStudyId, TestUserId.ToString(), TestReferenceMaterialId.ToString(), ImageFileName);
        protected static readonly string StudyResourceFullPath = FormatAsFilePath(TestStudyId, ImageFileName);

        protected Mock<IBlobStorageService> MockBlobStorageService;
        protected Mock<HttpPostedFileBase> MockHttpPostedFile;
        protected Mock<IKeyVaultService> MockKeyVaultService;
        protected Mock<IStudySettingService> MockStudySettingService;
        protected Mock<IServiceSettings> MockServiceSettings;
        protected MemoryStream ms;

        public BlobModel ExportBlobModel { get; private set; }
        public BlobModel PatientDataBlobModel { get; private set; }
        public BlobModel ReferenceMaterialBlobModel { get; private set; }
        public BlobModel ResourceFileBlobModel { get; private set; }
        public ExportDto TestExportDto { get; private set; }
        public ReferenceMaterialDto TestReferenceMaterialDto { get; private set; }

        [TestInitialize]
        public virtual void TestInitialize()
        {
            MockStudySettingService = new Mock<IStudySettingService>();
            MockServiceSettings = new Mock<IServiceSettings>();
            MockBlobStorageService = new Mock<IBlobStorageService>();
            MockKeyVaultService = new Mock<IKeyVaultService>();
            MockHttpPostedFile = Mock.Get(Mock.Of<HttpPostedFileBase>());

            MockServiceSettings
                .Setup(s => s.StudyId)
                .Returns(Guid.Parse(TestStudyId));

            TestExportDto = new ExportDto
            {
                Id = TestExportId,
                UserId = TestUserId
            };

            TestReferenceMaterialDto = new ReferenceMaterialDto()
            {
                Id = TestReferenceMaterialId,
                UserId = TestUserId,
                FileName = ImageFileName,
                File = MockHttpPostedFile.Object
            };

            Bitmap bm = new Bitmap(50, 50);
            ms = new MemoryStream();
            bm.Save(ms, ImageFormat.Png);

            ExportBlobModel = CreateTestBlobModel(ExportFullPath, ZipContentType, ms.ToArray());
            ReferenceMaterialBlobModel = CreateTestBlobModel(ReferenceMaterialFullPath, ImageContentType, ms.ToArray());
            ResourceFileBlobModel = CreateTestBlobModel(StudyResourceFullPath, ImageContentType, ms.ToArray());
            PatientDataBlobModel = CreateTestBlobModel(PatientDataFullPath, ImageContentType, ms.ToArray());

            MockBlobStorageService
                .Setup(bss => bss.DeleteBlob(
                    It.IsAny<string>(),
                    It.Is<string>(q => q == FileServiceConstants.ReferenceMaterialContainer),
                    It.Is<string>(q => q == ReferenceMaterialFullPath)))
                .ReturnsAsync(true);

            MockKeyVaultService.Setup(ks => ks.GetSecretValueFromKey(It.IsAny<string>()))
                .ReturnsAsync(TestConnectionString);

            MockHttpPostedFile.Setup(_ => _.FileName)
                .Returns(ImageFileName);
            MockHttpPostedFile.Setup(_ => _.InputStream)
                .Returns(ms);
        }

        protected BlobModel CreateTestBlobModel(string fileNameWithPath, string contentType, byte[] content)
        {
            return new BlobModel()
            {
                Content = content,
                ContentType = contentType,
                Name = fileNameWithPath
            };
        }

        protected async Task<IFileService> GetService()
        {
            var service = new FileService(
                MockBlobStorageService.Object,
                MockServiceSettings.Object,
                MockKeyVaultService.Object);

            return service;
        }

        protected void SetupGetBlobContents(string container, BlobModel blobModel)
        {
            MockBlobStorageService
                .Setup(bss => bss.GetBlobContents(
                    TestConnectionString,
                    container,
                    blobModel.Name))
                .ReturnsAsync(blobModel);
        }

        protected void SetupUploadBlob(string container, BlobModel blobModel, bool returnValue = true)
        {
            MockBlobStorageService
                .Setup(bss => bss.UploadBlob(
                    TestConnectionString,
                    container,
                    blobModel,
                    false))
                .ReturnsAsync(returnValue);
        }

        private static string FormatAsFilePath(params string[] pathItems)
        {
            var fullPath = string.Join(FileServiceConstants.VirtualDirectorySeparator, pathItems);

            return $"{FileServiceConstants.VirtualDirectorySeparator}{fullPath}";
        }
    }
}