using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using YPrime.Data.Study;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.BusinessLayer.Reports
{
    internal static class Helper
    {
        public static List<ReportDataDto> ToReportData<T>(this List<T> source, List<string> columns)
        {
            var reportData = new List<ReportDataDto>();
            var orderedColumns = columns.Select((x, index) => new {Column = x, Order = index});

            var properties = typeof(T).GetProperties().Join(orderedColumns, x => x.Name, y => y.Column,
                    (x, y) => new {Property = x, ColumnInfo = y})
                .OrderBy(x => x.ColumnInfo.Order)
                .Select(x => x.Property);

            foreach (var item in source)
            {
                var reportItem = new ReportDataDto {Row = new Dictionary<string, object>()};
                foreach (var prop in properties)
                {
                    reportItem.Row.Add(prop.Name, prop.GetValue(item));
                }

                reportData.Add(reportItem);
            }

            return reportData;
        }
    }
}