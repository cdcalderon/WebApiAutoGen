using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using YPrime.API.Models;
using YPrime.BusinessLayer.Exceptions;
using YPrime.BusinessLayer.Interfaces;
using YPrime.Config.Enums;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Data.Study.Models.Models.DataSync;

namespace YPrime.API.Controllers
{    
    public class DataSyncController : BaseApiController
    {
        private readonly IDeviceRepository _DeviceRepository;
        private readonly IDiaryEntryRepository _DiaryEntryRepository;
        private readonly IDataSyncRepository _SyncRepository;

        public DataSyncController(IDataSyncRepository SyncRepository,
            IDiaryEntryRepository DiaryEntryRepository,
            IDeviceRepository DeviceRepository,
            ISessionService sessionService) : base(sessionService)
        {
            _SyncRepository = SyncRepository;
            _DiaryEntryRepository = DiaryEntryRepository;
            _DeviceRepository = DeviceRepository;
        }

        [Route("api/DataSync/SyncClientData")]
        [Authorize]
        public async Task<DataSyncResponse> SyncClientData(DataSyncRequest deviceInfo)
        {
            var syncResponse = new DataSyncResponse();
            var syncLogMessage = string.Empty;

            try
            {
                var clientEntries = deviceInfo.ClientEntries;
                var devicePatientId = deviceInfo.PatientId;
                clientEntries = _DeviceRepository.GetAdditionalTableData(deviceInfo.DeviceId, clientEntries);

                syncResponse = await _SyncRepository.SyncClientData(
                    deviceInfo.DeviceId,
                    deviceInfo.DeviceTypeId,
                    deviceInfo.SoftwareVersion,
                    deviceInfo.ConfigurationVersion,
                    clientEntries,
                    deviceInfo.AuditEntries,
                    devicePatientId);

                if (syncResponse.Success)
                {
                    syncLogMessage = Enum.GetName(typeof(HttpStatusCode), 200);
                }
            }
            catch (BusinessException ex)
            {
                syncLogMessage = ex.Message;
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent(ex.Message)
                });
            }
            catch (Exception ex)
            {
                syncLogMessage = ex.Message;
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(ex.Message)
                });
            }
            finally
            {
                LogDeviceSyncData(
                    deviceInfo.ConfigurationVersion,
                    deviceInfo.DeviceId,
                    deviceInfo.SoftwareVersion,
                    nameof(SyncClientData), 
                    syncResponse.Success,
                    syncLogMessage,
                    deviceInfo);
            }

            return syncResponse;
        }

        public ResponseMessageResult GetToken(string Id)
        {
            var token = _DeviceRepository.GenerateToken(Id);
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                RequestMessage = Request,
                Content = new StringContent(token)
            };
            return ResponseMessage(httpResponseMessage);
        }

        [Route("api/DataSync/SyncInitialClientData")]        
        public async Task<DataSyncResponse> SyncInitialClientData(DataSyncRequest syncRequest)
        {
            var softwareVersion = syncRequest.SoftwareVersion;
            var configVersion = syncRequest.ConfigurationVersion;
            var syncResponse = new DataSyncResponse();
            var syncLogMessage = string.Empty;

            try
            {
                syncResponse = await _SyncRepository.SyncInitialData(
                    syncRequest.DeviceId,
                    syncRequest.DeviceTypeId,
                    syncRequest.SiteId,
                    syncRequest.PatientId,
                    softwareVersion,
                    configVersion,
                    syncRequest.ClientEntries);

                if (syncResponse.Success)
                {
                    syncLogMessage = Enum.GetName(typeof(HttpStatusCode), 200);
                    _SyncRepository.AddDeviceData(syncRequest.DeviceId, syncRequest.EncryptionKey);
                }
            }
            catch (BusinessException ex)
            {
                syncLogMessage = ex.Message;
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent(ex.Message)
                });
            }
            catch (Exception ex)
            {
                syncLogMessage = ex.Message;
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(ex.Message)
                });
            }
            finally
            {
                LogDeviceSyncData(
                    configVersion,
                    syncRequest.DeviceId,
                    softwareVersion,
                    nameof(SyncInitialClientData),
                    syncResponse.Success,
                    syncLogMessage,
                    syncRequest);
            }

            return syncResponse;
        }

        public CheckForUpdateResponse CheckForUpdates(CheckForUpdatesRequest checkForUpdatesRequest)
        {
            var deviceId = checkForUpdatesRequest.DeviceId;
            var deviceTypeId = checkForUpdatesRequest.DeviceTypeId;
            var softwareVersion = checkForUpdatesRequest.SoftwareVersion;
            var configVersion = checkForUpdatesRequest.ConfigVersion;
            var siteId = checkForUpdatesRequest.SiteId;
            var assetTag = checkForUpdatesRequest.AssetTag;
            var response = new CheckForUpdateResponse();
            bool success = false;
            var syncLogMessage = string.Empty;

            try
            {
                if (deviceTypeId != DeviceType.BYOD.Id)
                {
                    _SyncRepository.CreateDeviceIfNotExists(deviceId, deviceTypeId, siteId, softwareVersion, assetTag);
                }

                response = _SyncRepository.CheckForUpdates(deviceId, softwareVersion, configVersion);
                success = true;
                syncLogMessage = Enum.GetName(typeof(HttpStatusCode), 200);
            }
            catch (CheckForUpdatesException ex)
            {
                syncLogMessage = ex.Message;
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent(ex.Message)
                });
            }
            catch (Exception ex)
            {
                syncLogMessage = ex.Message;
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(ex.Message)
                });
            }
            finally
            {
                this.LogDeviceSyncData(configVersion, deviceId, softwareVersion, "CheckForUpdates", success, syncLogMessage, checkForUpdatesRequest);
            }

            return response;
        }

        private void LogDeviceSyncData(string configVersion, Guid deviceId, string softwareVersion, string syncAction, bool syncSuccess, string syncLogMessage, dynamic clientEntries)
        {
            _SyncRepository.LogDeviceSyncData(configVersion, deviceId, softwareVersion, syncAction, syncSuccess, syncLogMessage, clientEntries);
        }

        [Route("api/DataSync/UploadFile")]
        [Authorize]
        public async Task<bool> UploadFile([FromBody] FileUploadData fileUploadData)
        {
            if (fileUploadData.DiaryEntryId != null)
            {
                await _DiaryEntryRepository.SaveDiaryEntryImageToDisk((Guid) fileUploadData.DiaryEntryId,
                    fileUploadData.Base64DataUrl, fileUploadData.FileName);
            }

            // errors will return a non 200 result
            // returning true to maintain backwards compatability with
            // older versions of the device app
            return true;
        }
    }
}