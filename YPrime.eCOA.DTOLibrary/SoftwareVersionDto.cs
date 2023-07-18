using System;
using System.Web;

namespace YPrime.eCOA.DTOLibrary
{
    [Serializable]
    public class SoftwareVersionDto
    {
        public Guid Id { get; set; }

        public string VersionNumber { get; set; }

        public string PackageFileName { get; set; }

        public string PackagePath { get; set; }

        public Guid PlatformTypeId { get; set; }

        public string PlatformTypeName { get; set; }

        public DateTime UploadDate { get; set; }

        public HttpPostedFileBase ApkFile { get; set; }

        public Version Version { get; set; }
    }
}