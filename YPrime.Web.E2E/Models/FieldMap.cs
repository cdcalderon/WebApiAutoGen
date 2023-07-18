using YPrime.Web.E2E.Interfaces;

namespace YPrime.Web.E2E.Models
{
    public class FieldMap : IFieldMap
    {
        public string FieldType { get; set; }
        public string Label { get; set; }
        public string Value { get; set; }
        public string DefaultValue { get; set; }
        public string UiControl { get; set; }
        public string Id { get; set; }
        public string ValidationId { get; set; }
        public string FieldName { get; set; }
        public string State { get; set; }
        public int Position { get; set; }
        public string FieldContainerElement { get; set; }
        public bool WaitForElement { get; set; }
        public string LocatorType { get; set; } = "id";
        public string LocatorValue { get; set; } 
    }
}
