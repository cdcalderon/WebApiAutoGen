using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Web;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.BusinessLayer.Interfaces
{
    public interface IFileService
    {
        Task<byte[]> GetExportArchive(ExportDto exportDto);

        Task<byte[]> GetReferenceMaterial(ReferenceMaterialDto referenceMaterialDto);

        Task UploadReferenceMaterial(ReferenceMaterialDto referenceMaterialDto);

        Task DeleteReferenceMaterial(Guid userId, Guid Id, string fileName);

        Task<byte[]> GetResourceFile(string name);

        Task UploadResourceFile(HttpPostedFileBase file);

        Task CreateExportArchive(Guid userId, Guid exportId, List<ExportStream> exportStreams);

        Task SaveDiaryImage(string base64, Guid patientId, Guid? visitId, Guid questionnaireId, string FileName);

        Task<Image> GetDiaryAnswerImage(string fileName, Guid patientId, Guid? visitId, Guid questionnaireId);
        Task<Image> GetImageFromPath(string container, string fullFilePath, string fileName);
    }
}
