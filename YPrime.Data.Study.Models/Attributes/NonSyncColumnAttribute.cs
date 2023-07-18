using System;

namespace YPrime.Data.Study.Models.Attributes
{
    // This attribute is used to determine if during a device sync, specific columns should not be updated.
    // The need for this is currently the IRT completed date, which only gets updated on the server.
    [AttributeUsage(AttributeTargets.Property)]
    public class NonSyncColumnAttribute : Attribute
    {
    }
}