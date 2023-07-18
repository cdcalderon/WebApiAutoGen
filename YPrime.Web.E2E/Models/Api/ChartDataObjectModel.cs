using System;
using System.Collections.Generic;
using System.Text;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.Web.E2E.Models
{
    class ChartDataObjectModel
    {

        public Guid Id { get; set; }
        public string Title { get; set; }
        public string XAxisLabel { get; set; }
        public string YAxisLabel { get; set; }
        public Dictionary<double, string> XLabels { get; set; }
        public Dictionary<double, string> YLabels { get; set; }
        public object ChartObject { get; set; }
        public object ChartType { get; set; }
        public List<object> CustomChartTypes { get; set; }
        public object ChartSize { get; set; }
        public bool IncludeLegend { get; set; }
        public object LegendPosition { get; set; }
        public bool Is3dChart { get; set; }
        public List<ChartSeriesObject> ChartSeries { get; set; }
        public bool ShowDisplayTitle { get; set; }
        public bool HideContainer { get; set; }
        public string ChartSeriesMouseOverColor { get; set; }
    }
}
