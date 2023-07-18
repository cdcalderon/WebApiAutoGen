using System;
using System.Collections.Generic;
using System.Drawing;

namespace YPrime.eCOA.DTOLibrary
{
    [Serializable]
    public class ChartSeriesObject
    {
        public int SeriesOrder { get; set; }
        public List<Point> SeriesData { get; set; }
        public List<ChartDataPoint> SeriesDataPoints { get; set; }
        public string SeriesName { get; set; }
        public ChartSeriesStyle SeriesStyle { get; set; }
        public bool? HidePlotPoints { get; set; }
    }

    [Serializable]
    public class ChartDataPoint
    {
        public string HoverText { get; set; }
        public float X { get; set; }
        public string XLabel { get; set; }
        public float Y { get; set; }
        public string YLabel { get; set; }
        public object Id { get; set; }
    }

    [Serializable]
    public class ChartSeriesStyle
    {
        public string Fill { get; set; }
        public string Strokecolor { get; set; }
        public bool? Strokedash { get; set; }
        public string Strokeopacity { get; set; }
        public string Strokewidth { get; set; }
    }
}