using Config.Enums;
using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using YPrime.eCOA.DTOLibrary.WebBackup;

namespace YPrime.BusinessLayer.Interfaces
{
    public interface IWebBackupRepository
    {
        Task<WebBackupModel> GetClinicianWebBackupModel(Guid siteId, string hostAddress);

        Task<WebBackupModel> GetSubjectWebBackupModel(string token, string hostAddress);

        Task<WebBackupJwtModel> CreateJwtModel(Guid patientId, Guid siteId, 
            WebBackupType webBackupType, Guid? visitId);

        Task<WebBackupEmailModel> CreateWebBackupEmailModel(
            Guid patientId,
            Guid siteId,
            WebBackupType webBackupType = WebBackupType.HandheldPatient,
            Guid? visitId = null);

        Task<string> CreateWebBackupEmailBody(
            HttpRequestBase request,
            UrlHelper urlHelper,
            WebBackupJwtModel jwtModel);

        string GetWebBackupUrl(
            WebBackupType webBackupType,
            Guid deviceId,
            Guid deviceTypeId,
            Guid siteId,
            string assetTag,
            Guid? patientId,
            string timeZone,
            Guid? configId,
            Guid? visitId = null,
            Guid? caregiverId = null);

        Task<bool> IsWebBackupHandheldEnabled();

        Task<int> GetWebBackupHandheldValue();

        Task<int> GetWebBackupTabletValue();
    }
}