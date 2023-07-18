using System;
using System.Collections.Generic;
using System.Text;

namespace YPrime.Web.E2E.Interfaces
{
    public interface IFieldMap
    {
        string FieldType { get; set; }
        string Label { get; set; }
        string Value { get; set; }
        string DefaultValue { get; set; }
        string UiControl { get; set; }
        string Id { get; set; }
        string ValidationId { get; set; }
        string FieldName { get; set; }
    }
}
