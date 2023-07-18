using System;
using System.Collections.Generic;
using System.Text;
using TechTalk.SpecFlow.Assist.Attributes;

namespace YPrime.Web.E2E.Models
{
    public class SoftwareReleaseGridMap
    {
        public string ReleaseDate { get; set; }
        public string ReleaseName { get; set; }
        public string SoftwareVersion { get; set; }
        public string Active { get; set; }
        public string Required { get; set; }
        public string ConfigurationVersion { get; set; }
        public string StudyWide { get; set; }
        public string DeviceType { get; set; }
        [TableAliases("Country(s)")]
        public string Countrys { get; set; }
        [TableAliases("Sites(s)")]
        public string Sites { get; set; }
        [TableAliases("Devices(s)")]
        public string Devices { get; set; }
        [TableAliases("Assigned/Reported Config")]
        public string AssignedReportedConfig { get; set; }
        [TableAliases("Assigned/Reported Software")]
        public string AssignedReportedSoftware { get; set; }

    }
}
