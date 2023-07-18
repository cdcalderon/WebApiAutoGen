using System.Collections.Generic;

namespace YPrime.StudyPortal.Models.DataTables
{
    public class DataTableAjaxPostModel
    {
        public int Draw { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        public List<Column> Columns { get; set; }
        public Search Search { get; set; }
        public List<Order> Order { get; set; }
    }
}