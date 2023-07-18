using System;
using System.Collections.Generic;

namespace YPrime.eCOA.DTOLibrary
{
    [Serializable]
    public class ReportDataDto
    {
        public ReportDataDto()
        {
            Row = new Dictionary<string, object>();
            CustomCellTextColors = new Dictionary<string, string>();
        }

        public object this[string key] => Row[key];

        public Dictionary<string, object> Row { get; set; }

        public Dictionary<string, string> CustomCellTextColors { get; set; }
        public string RowBackgroundColor { get; set; }
        public bool IsSummaryRow { get; set; }

        public object GetValue(string key)
        {
            return Row[key];
        }
    }
}