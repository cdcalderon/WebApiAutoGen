using System;

namespace YPrime.BusinessLayer.Constants
{
    public enum CorrectionCRUDTypes
    {
        Insert,
        Update,
        Delete
    }

    public static class CorrectionActionEnum
    {
        public static readonly Guid Approved = Guid.Parse("E0B99BCB-0F09-4A65-A06D-7E48F5C41808");
        public static readonly Guid Rejected = Guid.Parse("0EF32937-FE9F-4D97-B345-701080F0CFAE");
        public static readonly Guid NeedsMoreInformation = Guid.Parse("70DA4CC0-ACE7-45F1-BF8A-DB720131A601");
        public static readonly Guid Pending = Guid.Parse("D3C17321-C013-4D9D-8639-E2E9CCFA6ADF");
    }

    public static class CorrectionStatusEnum
    {
        public static readonly Guid Pending = Guid.Parse("5BDF72E8-D72C-45FA-ABE2-166CBC3520C7");
        public static readonly Guid Completed = Guid.Parse("14FC9304-7684-4BB1-8F1D-3B21302BE582");
        public static readonly Guid InProgress = Guid.Parse("899D9619-0FD3-4525-ADED-62EEF2A84CF9");
        public static readonly Guid Rejected = Guid.Parse("BB727B01-9FBF-4ACC-9E4F-F091ECFA44A6");
        public static readonly Guid NeedsMoreInformation = Guid.Parse("6463E9BD-B152-4449-ADB8-4172D5E1885E");
    }

    public static class CorrectionActionElectronicSignatureEnum
    {
        public static readonly string Approved = "approving";
        public static readonly string Rejected = "rejecting";
        public static readonly string NeedsMoreInformation = "requiring additional information for";
        public static readonly string Pending = "submitting";
    }
}