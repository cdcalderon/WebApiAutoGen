using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms.DataVisualization.Charting;

namespace YPrime.eCOA.DTOLibrary
{
    [Serializable]
    public class ChartDataObject
    {
        public enum CustomSeriesChartType
        {
            //
            // Summary:
            //     Point chart type.
            Point = 0,

            //
            // Summary:
            //     FastPoint chart type.
            FastPoint = 1,

            //
            // Summary:
            //     Bubble chart type.
            Bubble = 2,

            //
            // Summary:
            //     Line chart type.
            Line = 3,

            //
            // Summary:
            //     Spline chart type.
            Spline = 4,

            //
            // Summary:
            //     StepLine chart type.
            StepLine = 5,

            //
            // Summary:
            //     FastLine chart type.
            FastLine = 6,

            //
            // Summary:
            //     Bar chart type.
            Bar = 7,

            //
            // Summary:
            //     Stacked bar chart type.
            StackedBar = 8,

            //
            // Summary:
            //     Hundred-percent stacked bar chart type.
            StackedBar100 = 9,

            //
            // Summary:
            //     Column chart type.
            Column = 10,

            //
            // Summary:
            //     Stacked column chart type.
            StackedColumn = 11,

            //
            // Summary:
            //     Hundred-percent stacked column chart type.
            StackedColumn100 = 12,

            //
            // Summary:
            //     Area chart type.
            Area = 13,

            //
            // Summary:
            //     Spline area chart type.
            SplineArea = 14,

            //
            // Summary:
            //     Stacked area chart type.
            StackedArea = 15,

            //
            // Summary:
            //     Hundred-percent stacked area chart type.
            StackedArea100 = 16,

            //
            // Summary:
            //     Pie chart type.
            Pie = 17,

            //
            // Summary:
            //     Doughnut chart type.
            Doughnut = 18,

            //
            // Summary:
            //     Stock chart type.
            Stock = 19,

            //
            // Summary:
            //     Candlestick chart type.
            Candlestick = 20,

            //
            // Summary:
            //     Range chart type.
            Range = 21,

            //
            // Summary:
            //     Spline range chart type.
            SplineRange = 22,

            //
            // Summary:
            //     RangeBar chart type.
            RangeBar = 23,

            //
            // Summary:
            //     Range column chart type.
            RangeColumn = 24,

            //
            // Summary:
            //     Radar chart type.
            Radar = 25,

            //
            // Summary:
            //     Polar chart type.
            Polar = 26,

            //
            // Summary:
            //     Error bar chart type.
            ErrorBar = 27,

            //
            // Summary:
            //     Box plot chart type.
            BoxPlot = 28,

            //
            // Summary:
            //     Renko chart type.
            Renko = 29,

            //
            // Summary:
            //     ThreeLineBreak chart type.
            ThreeLineBreak = 30,

            //
            // Summary:
            //     Kagi chart type.
            Kagi = 31,

            //
            // Summary:
            //     PointAndFigure chart type.
            PointAndFigure = 32,

            //
            // Summary:
            //     Funnel chart type.
            Funnel = 33,

            //
            // Summary:
            //     Pyramid chart type.
            Pyramid = 34,

            FillGauge = 35
        }


        public ChartDataObject()
        {
            XLabels = new Dictionary<double, string>();
            YLabels = new Dictionary<double, string>();
            LegendPosition = Docking.Bottom;
            ChartSeries = new List<ChartSeriesObject>();
            ChartSeriesMouseOverColor = "blue";
            Is3dChart = false;
            Id = Guid.NewGuid();
            ShowDisplayTitle = true;
            HideContainer = false;
            IncludeLegend = true;
        }

        public Guid Id { get; set; }
        public string Title { get; set; }
        public string XAxisLabel { get; set; }
        public string YAxisLabel { get; set; }
        public Dictionary<double, string> XLabels { get; set; }
        public Dictionary<double, string> YLabels { get; set; }
        public object ChartObject { get; set; }
        public SeriesChartType ChartType { get; set; }
        public List<CustomSeriesChartType> CustomChartTypes { get; set; }
        public Size ChartSize { get; set; }
        public bool IncludeLegend { get; set; }
        public Docking LegendPosition { get; set; }
        public bool Is3dChart { get; set; }
        public List<ChartSeriesObject> ChartSeries { get; set; }
        public bool ShowDisplayTitle { get; set; }
        public bool HideContainer { get; set; }
        public string ChartSeriesMouseOverColor { get; set; }
    }
}