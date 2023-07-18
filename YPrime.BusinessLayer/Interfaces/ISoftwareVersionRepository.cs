using System;
using System.Collections.Generic;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.BusinessLayer.Interfaces
{
    public interface ISoftwareVersionRepository
    {
        List<SoftwareVersion> GetAllSoftwareVersions();
        IEnumerable<Guid> GetSoftwareVersionIdsAssignedToReleases();
        bool AddSoftwareVersion(SoftwareVersion sw);
        void DeleteSoftwareVersionById(Guid id);
        bool CheckVersionNumberIsUsed(string versionNumber);
        Version convertToVersion(string strVersion);
        string SaveToCDNFolder(SoftwareVersionDto softwareVersion, Uri websiteURL);
    }
}