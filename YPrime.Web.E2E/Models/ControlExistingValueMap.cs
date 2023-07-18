namespace YPrime.Web.E2E.Models
{
    public class ControlExistingValueMap
    {
        public string Label { get; set; }
        public string CurrentValue { get; set; }
        public string RequestedValue { get; set; }
        public string Fieldtype { get; set; }
        public string DateTimeFormat { get; set; }
        public bool? IsEnabled { get; set; }
        public bool? IsRemoveEnabled { get; set; }
    }
}
