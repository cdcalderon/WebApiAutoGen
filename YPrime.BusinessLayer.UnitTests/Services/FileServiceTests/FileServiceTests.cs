using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using YPrime.BusinessLayer.Constants;
using YPrime.eCOA.DTOLibrary;
using YPrime.StorageService.Services.Exceptions;
using YPrime.StorageService.Services.Models;

namespace YPrime.BusinessLayer.UnitTests.Services.FileServiceTests
{
    [TestClass]
    public class FileServiceTests : FileServiceTestBase
    {
        private static BlobModel resultBlob;

        [TestMethod]
        public async Task GetExportArchiveTest()
        {
            var service = await GetService();

            SetupGetBlobContents(FileServiceConstants.ExportDataContainer, ExportBlobModel);

            var result = await service.GetExportArchive(TestExportDto);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GetReferenceMaterialTest()
        {
            var service = await GetService();

            SetupGetBlobContents(FileServiceConstants.ReferenceMaterialContainer, ReferenceMaterialBlobModel);

            var result = await service.GetReferenceMaterial(TestReferenceMaterialDto);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task UploadReferenceMaterialTest()
        {
            var service = await GetService();

            SetupUploadBlob(FileServiceConstants.ReferenceMaterialContainer, ReferenceMaterialBlobModel);

            await service.UploadReferenceMaterial(TestReferenceMaterialDto);
            MockBlobStorageService
                .Verify(bss => bss.UploadBlob(
                    TestConnectionString,
                    FileServiceConstants.ReferenceMaterialContainer,
                    It.Is<BlobModel>(q => q.Name.Equals(ReferenceMaterialFullPath, StringComparison.OrdinalIgnoreCase)),
                    false),
                Times.Once);
        }

        [TestMethod]
        public async Task DeleteReferenceMaterialTest()
        {
            var service = await GetService();

            await service.DeleteReferenceMaterial(TestUserId, TestReferenceMaterialId, ImageFileName);
            MockBlobStorageService
                .Verify(bss => bss.DeleteBlob(
                    TestConnectionString,
                    FileServiceConstants.ReferenceMaterialContainer,
                    ReferenceMaterialFullPath),
                Times.Once);
        }

        [TestMethod]
        public async Task GetResourceFileTest()
        {
            var service = await GetService();

            SetupGetBlobContents(FileServiceConstants.StudyResourceContainer, ResourceFileBlobModel);

            var result = await service.GetResourceFile(ImageFileName);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task UploadResourceFileTest()
        {
            var service = await GetService();

            await service.UploadResourceFile(MockHttpPostedFile.Object);

            MockBlobStorageService
                .Verify(bss => bss.UploadBlob(
                    TestConnectionString,
                    FileServiceConstants.StudyResourceContainer,
                    It.Is<BlobModel>(q => q.Name.Equals(StudyResourceFullPath, StringComparison.OrdinalIgnoreCase)),
                    false),
                Times.Once);
        }

        [TestMethod]
        public async Task CreateExportArchiveTest()
        {
            var service = await GetService();

            List<ExportStream> esList = new List<ExportStream>() {
                new ExportStream(){
                    Name = "zipfile",
                    Extension = "zip",
                    MemoryStream = ms
                }
            };

            await service.CreateExportArchive(TestUserId, TestExportId, esList);

            MockBlobStorageService
                .Verify(bss => bss.UploadBlob(
                    TestConnectionString,
                    FileServiceConstants.ExportDataContainer,
                    It.Is<BlobModel>(q => q.Name.Equals(ExportFullPath, StringComparison.OrdinalIgnoreCase)),
                    false),
                Times.Once);
        }

        [TestMethod]
        public async Task SaveDiaryImageTest()
        {
            var service = await GetService();

            SetupUploadBlob(FileServiceConstants.PatientDataContainer, PatientDataBlobModel);

            await service.SaveDiaryImage(Convert.ToBase64String(ms.ToArray()), TestPatientId, TestVisitId, TestQuestionnaireId, ImageFileName);

            MockBlobStorageService
                .Verify(bss => bss.UploadBlob(
                    TestConnectionString,
                    FileServiceConstants.PatientDataContainer,
                    It.Is<BlobModel>(q => q.Name.Equals(PatientDataFullPath, StringComparison.OrdinalIgnoreCase)),
                    false),
                Times.Once);
        }

        [TestMethod]
        public async Task GetDiaryAnswerImageTest()
        {
            var service = await GetService();
            SetupGetBlobContents(FileServiceConstants.PatientDataContainer, PatientDataBlobModel);

            var result = await service.GetDiaryAnswerImage(ImageFileName, TestPatientId, TestVisitId, TestQuestionnaireId);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task SaveAndRetrieveDiaryAnswerImageTest_ReturnsValidImage()
        {
            SetupImageResult(FileServiceConstants.PatientDataContainer, TestPatientId, TestVisitId, TestQuestionnaireId);

            var service = await GetService();

            await Assert.ThrowsExceptionAsync<EntityDoesNotExistException>(async () =>
            {
                await service.GetDiaryAnswerImage(ImageFileName, TestPatientId, TestVisitId, TestQuestionnaireId);
            });

            await service.SaveDiaryImage(Convert.ToBase64String(ms.ToArray()), TestPatientId, TestVisitId, TestQuestionnaireId, ImageFileName);

            var expectedImageResult = await service.GetDiaryAnswerImage(ImageFileName, TestPatientId, TestVisitId, TestQuestionnaireId);
            Assert.IsNotNull(expectedImageResult);
        }

        [TestMethod]
        public async Task GetImageFromPathTest()
        {
            var filePath = string.Format("{0}Uploads{0}", FileServiceConstants.VirtualDirectorySeparator);
            var service = await GetService();

            var imageBlob = CreateTestBlobModel($"{filePath}{ImageFileName}", ImageContentType, ms.ToArray());
            SetupGetBlobContents(FileServiceConstants.PatientDataContainer, imageBlob);

            var result = await service.GetImageFromPath(FileServiceConstants.PatientDataContainer, filePath, ImageFileName);
            Assert.IsNotNull(result);
        }

        private void SetupImageResult(string container, Guid patientId, Guid visitId, Guid questionnaireId)
        {
            resultBlob = null;

            MockBlobStorageService
                .Setup(bss => bss.GetBlobContents(
                    TestConnectionString,
                    container,
                    PatientDataFullPath))
                .Callback(() =>
                {
                    if (resultBlob == null)
                    {
                        throw new EntityDoesNotExistException(ImageFileName);
                    }
                })
                .ReturnsAsync(() =>
                {
                    // Ensure we're returning the current copy of the resultBlob
                    return resultBlob;
                });

            MockBlobStorageService
                .Setup(bss => bss.UploadBlob(
                    TestConnectionString,
                    container,
                    It.Is<BlobModel>(q => q.Name.Equals(PatientDataFullPath, StringComparison.OrdinalIgnoreCase)),
                    false))
                .Callback(() =>
                {
                    resultBlob = PatientDataBlobModel;
                })
                .ReturnsAsync(true);
        }
    }
}