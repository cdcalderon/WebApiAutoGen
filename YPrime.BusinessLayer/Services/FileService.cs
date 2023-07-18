using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Web;
using Elmah;
using YPrime.BusinessLayer.Constants;
using YPrime.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.eCOA.DTOLibrary;
using YPrime.StorageService.Services.Exceptions;
using YPrime.StorageService.Services.Interfaces;
using YPrime.StorageService.Services.Models;

namespace YPrime.BusinessLayer.Services
{
    public class FileService : IFileService
    {
        private readonly IBlobStorageService _blobStorageService;
        private readonly IKeyVaultService _keyVaultService;
        private readonly string _studyId;

        private string _connectionString = null;

        public FileService(
            IBlobStorageService blobStorageService,
            IServiceSettings serviceSettings,
            IKeyVaultService keyVaultService)
        {
            _blobStorageService = blobStorageService;
            _keyVaultService = keyVaultService;

            _studyId = serviceSettings?.StudyId.ToString();
        }

        public async Task<byte[]> GetExportArchive(ExportDto exportDto)
        {
            var userArchivePath = GetFilePath(new string[] { _studyId, exportDto.UserId.ToString() });
            var connectionString = await GetConnectionString();
            var result = await _blobStorageService.GetBlobContents(connectionString, FileServiceConstants.ExportDataContainer, userArchivePath + $"{exportDto.Id}.zip");
            return result.Content;
        }

        public async Task<byte[]> GetReferenceMaterial(ReferenceMaterialDto referenceMaterialDto)
        {
            var userReferenceMaterialPath = GetFilePath(new string[] { _studyId, referenceMaterialDto.UserId.ToString(), referenceMaterialDto.Id.ToString() });
            var connectionString = await GetConnectionString();
            var result = await _blobStorageService.GetBlobContents(connectionString, FileServiceConstants.ReferenceMaterialContainer, userReferenceMaterialPath + referenceMaterialDto.FileName);
            return result.Content;
        }

        public async Task UploadReferenceMaterial(ReferenceMaterialDto referenceMaterialDto)
        {
            var fullFilePath = GetFilePath(new string[] { _studyId, referenceMaterialDto.UserId.ToString(), referenceMaterialDto.Id.ToString() });
            var stream = referenceMaterialDto.File.InputStream;

            var blobModel = new BlobModel()
            {
                Content = GetByteArray(stream),
                ContentType = MimeMapping.GetMimeMapping(referenceMaterialDto.File.FileName),
                Name = fullFilePath + referenceMaterialDto.File.FileName
            };

            var connectionString = await GetConnectionString();
            await _blobStorageService.UploadBlob(connectionString, FileServiceConstants.ReferenceMaterialContainer, blobModel);
        }

        public async Task DeleteReferenceMaterial(Guid userId, Guid Id, string fileName)
        {
            var fullFilePath = GetFilePath(new string[] { _studyId, userId.ToString(), Id.ToString() });
            var connectionString = await GetConnectionString();
            await _blobStorageService.DeleteBlob(connectionString, FileServiceConstants.ReferenceMaterialContainer, fullFilePath + fileName);
        }

        public async Task<byte[]> GetResourceFile(string name)
        {
            var studyResourceFilePath = GetFilePath(new string[] { _studyId });
            var connectionString = await GetConnectionString();
            var result = await _blobStorageService.GetBlobContents(connectionString, FileServiceConstants.StudyResourceContainer, studyResourceFilePath + name);
            return result.Content;
        }

        public async Task UploadResourceFile(HttpPostedFileBase file)
        {
            var fullFilePath = GetFilePath(new string[] { _studyId });
            await UploadFile(FileServiceConstants.StudyResourceContainer, fullFilePath, file);
        }

        public async Task CreateExportArchive(Guid userId, Guid exportId, List<ExportStream> exportStreams)
        {
            var fullZipFilePath = GetFilePath(new string[] { _studyId, userId.ToString() });
            var filename = $"{exportId}.zip";
            await CreateZipArchive(FileServiceConstants.ExportDataContainer, fullZipFilePath, filename, exportStreams);
        }

        private async Task UploadFile(string container, string path, HttpPostedFileBase file)
        {
            var stream = file.InputStream;

            var blobModel = new BlobModel()
            {
                Content = GetByteArray(stream),
                ContentType = MimeMapping.GetMimeMapping(file.FileName),
                Name = path + file.FileName
            };

            var connectionString = await GetConnectionString();
            await _blobStorageService.UploadBlob(connectionString, container, blobModel);
        }

        private async Task CreateZipArchive(string container, string zipFilePath, string fileName, List<ExportStream> exportStreams)
        {
            var connectionString = await GetConnectionString();
           
            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var exportStream in exportStreams)
                    {
                        var zipArchive = archive.CreateEntry($"{exportStream.Name}.{exportStream.Extension}");
                        using (var exportFileStream = zipArchive.Open())
                        {
                            exportStream.MemoryStream.Seek(0, SeekOrigin.Begin);
                            exportStream.MemoryStream.CopyTo(exportFileStream);
                        }
                    }
                }

                var blobModel = new BlobModel()
                {
                    Content = GetByteArray(memoryStream),
                    ContentType = "application/zip",
                    Name = zipFilePath + fileName
                };

                try
                {
                    await _blobStorageService.UploadBlob(connectionString, container, blobModel);
                }
                catch (BlobExistsException ex)
                {
                    // If the blob already exists, log it but don't return an exception to the portal for exports
                    // This also prevents hangfire from keep scheduling "failed" jobs for this reason
                    if (HttpContext.Current != null)
                    {
                        ErrorSignal.FromCurrentContext().Raise(ex);
                    }
                }
            }
        }

        public async Task SaveDiaryImage(string base64, Guid patientId, Guid? visitId, Guid questionnaireId, string FileName)
        {
            var filePath = GetFilePath(new string[] { _studyId, patientId.ToString(), visitId.ToString(), questionnaireId.ToString() });
            var blobModel = new BlobModel()
            {
                Content = Convert.FromBase64String(base64),
                ContentType = MimeMapping.GetMimeMapping(FileName),
                Name = filePath + FileName
            };

            var connectionString = await GetConnectionString();

            try
            {
                await _blobStorageService.UploadBlob(connectionString, FileServiceConstants.PatientDataContainer, blobModel);
            } 
            catch (BlobExistsException ex)
            {
                // If the blob already exists, log it but don't return an exception to the device
                if (HttpContext.Current != null)
                {
                    ErrorSignal.FromCurrentContext().Raise(ex);
                }
            }
        }

        public async Task<Image> GetDiaryAnswerImage(string fileName, Guid patientId, Guid? visitId, Guid questionnaireId)
        {
            var filePath = GetFilePath(new string[] { _studyId, patientId.ToString(), visitId.ToString(), questionnaireId.ToString() });
            return await GetImageFromPath(FileServiceConstants.PatientDataContainer, filePath, fileName);
        }

        public async Task<Image> GetImageFromPath(string container, string fullFilePath, string fileName)
        {
            Image result;

            var connectionString = await GetConnectionString();
            var blob = await _blobStorageService.GetBlobContents(connectionString, container, fullFilePath + fileName);
            var stream = GetStreamFromByteArray(blob.Content);

            if (stream.Length > 0)
            {
                result = Image.FromStream(stream);
            }
            else
            {
                result = null;
            }

            return result;
        }

        private MemoryStream GetStreamFromByteArray(byte[] content)
        {
            MemoryStream memoryStream = new MemoryStream(content);
            memoryStream.Position = 0;
            return memoryStream;
        }

        private byte[] GetByteArray(Stream stream)
        {
            var br = new BinaryReader(stream);
            long numBytes = stream.Length;
            br.BaseStream.Position = 0;
            byte[] buff = br.ReadBytes((int)numBytes);
            return buff;
        }

        private string GetFilePath(string[] values)
        {
            string filePath = FileServiceConstants.VirtualDirectorySeparator;
            foreach (var v in values)
            {
                filePath += v + FileServiceConstants.VirtualDirectorySeparator;
            }

            return filePath;
        }

        private async Task<string> GetConnectionString()
        {
            if (string.IsNullOrWhiteSpace(_connectionString))
            {
                _connectionString = await _keyVaultService.GetSecretValueFromKey(FileServiceConstants.FileStoreSecretKey);
            }

            return _connectionString;
        }
    }
}