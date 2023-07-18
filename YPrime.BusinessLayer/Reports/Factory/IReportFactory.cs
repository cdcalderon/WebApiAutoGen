using System;
using YPrime.BusinessLayer.Enums;

namespace YPrime.BusinessLayer.Reports.Factory
{
    public interface IReportFactory
    {
        IReport CreateReport(ReportType reportType);
    }
}