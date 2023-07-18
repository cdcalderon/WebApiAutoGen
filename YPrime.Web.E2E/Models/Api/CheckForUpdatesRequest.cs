﻿using System;
using System.Collections.Generic;
using System.Text;

namespace YPrime.Web.E2E.Models.Api
{
    public class CheckForUpdatesRequest
    {
        public Guid DeviceId { get; set; }

        public Guid DeviceTypeId { get; set; }

        public string AssetTag { get; set; }

        public Guid SiteId { get; set; }

        public string SoftwareVersion { get; set; }

        public string ConfigVersion { get; set; }

        public ulong FreeSpace { get; set; }
    }
}
