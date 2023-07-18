using System;
using System.Collections.Generic;
using System.Web.Mvc;
using YPrime.Core.BusinessLayer.Models;

namespace YPrime.eCOA.DTOLibrary
{
    [Serializable]
    public class SoftwareReleaseDto : DtoBase
    {
        public Guid Id { get; set; }
        public Guid SoftwareVersionId { get; set; }
        public string ReleaseDate { get; set; }
        public string Name { get; set; }
        public string VersionNumber { get; set; }
        public string ConfigVersionNumber { get; set; }
        public bool IsActive { get; set; }
        public bool Required { get; set; }
        public bool StudyWide { get; set; }
        public SelectList VersionList { get; set; }
        public SelectList ConfigVersionList { get; set; }
        public List<Guid> DeviceTypeIds { get; set; } = new List<Guid>();
        public SelectList DeviceTypeList { get; set; }
        public string DeviceTypeNames { get; set; }
        public string AssetTagList { get; set; }
        public List<SiteDto> Sites { get; set; }
        public List<Guid> SiteIds { get; set; }
        public string SiteNameList { get; set; }
        public List<CountryModel> Countries { get; set; }
        public List<Guid> CountryIds { get; set; }
        public string CountryNameList { get; set; }
        public string AssignedReportedVersionCount { get; set; }
        public string AssignedReportedConfigCount { get; set; }
        public string ViewDevices { get; set; }
        public Guid ConfigurationId { get; set; }
        public List<ConfigurationVersion> ConfigurationVersions { get; set; } = new List<ConfigurationVersion>();
    }
}