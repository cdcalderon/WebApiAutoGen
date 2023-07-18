using System;
using YPrime.BusinessLayer.Interfaces;

namespace YPrime.BusinessLayer.DataSync.Factories
{
    public interface ISqlBuilderFactory
    {
        ISyncSQLBuilder Build(
            Guid? devicePatientId,
            Guid deviceId, 
            Guid deviceTypeId, 
            bool initialSync);
    }
}
