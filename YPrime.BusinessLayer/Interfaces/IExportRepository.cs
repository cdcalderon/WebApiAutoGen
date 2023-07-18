using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YPrime.eCOA.DTOLibrary;

namespace YPrime.BusinessLayer.Interfaces
{
    public interface IExportRepository
    {
        Task<ExportDto> GetExport(Guid Id);

        Task<IEnumerable<ExportDto>> GetExports(Guid userId);

        void CreateExport(ExportDto exportDto);

        Task<List<ExportStream>> ExecuteExport(Guid exportId);

        bool ExportExists(string name);

        List<ExportStream> ToExportFiles(DbDataReader dataReader, bool includeHeaderAsFirstRow, Encoding Encoding,
            string Delimeter, string Extension);
    }
}