using System;
using System.Linq;
using YPrime.BusinessLayer.Exceptions;
using YPrime.BusinessLayer.Interfaces;
using YPrime.BusinessLayer.SyncSQLBuilders;
using YPrime.Data.Study;

namespace YPrime.BusinessLayer.DataSync.Factories
{
    public class SqlBuilderFactory : ISqlBuilderFactory
    {
        private readonly IStudyDbContext _dbContext;

        public SqlBuilderFactory(
            IStudyDbContext dbContext) 
        {
            _dbContext = dbContext;
        }

        public ISyncSQLBuilder Build(
            Guid? devicePatientId,
            Guid deviceId,
            Guid deviceTypeId,
            bool initialSync)
        {
            ISyncSQLBuilder builder;

            switch (deviceTypeId)
            {
                case var d when d == Config.Enums.DeviceType.BYOD.Id:

                    var patientId = initialSync ? _dbContext
                    .Devices
                    .FirstOrDefault(dv => dv.Id == deviceId)?.PatientId : devicePatientId;

                    if (patientId == null)
                    {
                        throw new PatientNotFoundException();
                    }

                    builder = new SQLBuilderBYOD(
                    _dbContext,
                    patientId.ToString());
                    break;

                case var d when d == Config.Enums.DeviceType.Tablet.Id:

                    var syncingDevice = _dbContext
                    .Devices
                    .FirstOrDefault(dv => dv.Id == deviceId);

                    if (syncingDevice == null)
                    {
                        throw new DeviceNotFoundException();
                    }

                    builder = new SQLBuilderSite(
                    _dbContext,
                    syncingDevice.SiteId.ToString());
                    break;

                case var d when d == Config.Enums.DeviceType.Phone.Id:

                    if (devicePatientId == null)
                    {
                        var device = _dbContext
                        .Devices
                        .FirstOrDefault(dv => dv.Id == deviceId);

                        if (device == null)
                        {
                            throw new DeviceNotFoundException();
                        }

                        builder = new SQLBuilderPatientInitial(
                        _dbContext,
                        deviceId.ToString(),
                        device.SiteId.ToString());
                    }
                    else
                    {
                        builder = new SQLBuilderPatient(
                        _dbContext,
                        devicePatientId?.ToString() ?? string.Empty);
                    }
                    break;

                default:
                    throw new DeviceTypeNotFoundException();
            }

            return builder;
        }
    }
}
