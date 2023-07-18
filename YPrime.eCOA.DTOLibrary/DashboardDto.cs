using System;
using System.Collections.Generic;
using YPrime.Data.Study.Models;

namespace YPrime.eCOA.DTOLibrary
{
    [Serializable]
    public class DashboardDto
    {
        public string SaveDashboardApiUrl { get; set; }
        public string RenderWidgetApiUrl { get; set; }
        public List<Widget> Widgets { get; set; }
        public List<Widget> AvailableWidgets { get; set; }
        public int MaxWidgets { get; set; }
    }
}