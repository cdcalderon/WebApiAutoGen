using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using YPrime.BusinessLayer.BaseClasses;
using YPrime.BusinessLayer.Interfaces;
using YPrime.BusinessLayer.Session;
using YPrime.Data.Study;
using YPrime.Data.Study.Models;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.BusinessLayer.Repositories
{
    public class SoftwareVersionRepository : BaseRepository, ISoftwareVersionRepository
    {
        private readonly string APKFileExtension = ".apk";
        private readonly string APKPrefix = "Yprime.eCOA.Droid_";

        public SoftwareVersionRepository(IStudyDbContext db) : base(db)
        {
        }

        public List<SoftwareVersion> GetAllSoftwareVersions()
        {
            return _db.SoftwareVersions.ToList();
        }

        public Version convertToVersion(string strVersion)
        {
            try
            {
                return Version.Parse(strVersion);
            }
            catch
            {
                return null;
            }
        }

        public bool AddSoftwareVersion(SoftwareVersion newSoftwareVersion)
        {
            var result = false;

            var entity = _db.SoftwareVersions.SingleOrDefault(d => d.Id == newSoftwareVersion.Id);

            if (entity == null)
            {
                _db.SoftwareVersions.Add(newSoftwareVersion);
            }
            else
            {
                Exception e = new Exception("Incorrect input!");
                throw (e);
            }

            _db.SaveChanges(YPrimeSession.Instance?.CurrentUser?.Id.ToString());

            result = true;

            return result;
        }

        public bool CheckVersionNumberIsUsed(string versionNumber)
        {
            return _db.SoftwareVersions.Any(x => x.VersionNumber == versionNumber);
        }

        public string SaveToCDNFolder(SoftwareVersionDto softwareVersion, Uri websiteURL)
        {
            var siteUrl = websiteURL.ToString();

            string filename = (APKPrefix + softwareVersion.VersionNumber + APKFileExtension);

            var sponsorPath = Path.Combine(HostingEnvironment.MapPath("~/"), YPrimeSession.Instance.Sponsor);

            if (!Directory.Exists(sponsorPath))
            {
                Directory.CreateDirectory(sponsorPath);
            }

            var protocolPath = Path.Combine(HostingEnvironment.MapPath("~/"),
                YPrimeSession.Instance.Sponsor + @"\\" + YPrimeSession.Instance.Protocol);

            if (!Directory.Exists(protocolPath))
            {
                Directory.CreateDirectory(protocolPath);
            }

            var filePath = YPrimeSession.Instance.Sponsor + @"\" + YPrimeSession.Instance.Protocol + @"\" + filename;

            var filePathURL = YPrimeSession.Instance.Sponsor + @"/" + YPrimeSession.Instance.Protocol + @"/" + filename;

            var pathLocal = Path.Combine(HostingEnvironment.MapPath("~/"), filePath);

            var pathUrl = siteUrl + filePathURL;

            softwareVersion.ApkFile.SaveAs(pathLocal);

            softwareVersion.PackagePath = pathUrl;
            softwareVersion.ApkFile.SaveAs(pathLocal);

            return pathUrl;
        }

        public void DeleteSoftwareVersionById(Guid id)
        {
            var softwareVersion = _db.SoftwareVersions.Single(v => v.Id == id);

            _db.SoftwareVersions.Remove(softwareVersion);
            _db.SaveChanges(YPrimeSession.Instance?.CurrentUser?.Id.ToString());
        }

        public IEnumerable<Guid> GetSoftwareVersionIdsAssignedToReleases()
        {
            var versionIds = _db.SoftwareReleases.Select(s => s.SoftwareVersion.Id).Distinct();

            return versionIds;
        }
    }
}