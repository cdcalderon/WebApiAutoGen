using System;

namespace YPrime.BusinessLayer.Constants
{
    public static class EmailTypes
    {
        public static readonly Guid DataCorrection = new Guid("9C6E5203-83D9-48BE-96C8-0253F2B664BA");
        public static readonly Guid DataCorrectionApproved = new Guid("9C6E5203-83D9-48BE-96C8-0253F2B664BA");
        public static readonly Guid DataCorrectionPendingApproval = new Guid("2730FE90-6191-4E49-8D89-6A82232B4DFF");
        public static readonly Guid DataCorrectionRejected = new Guid("4C4D58DE-44FD-4AC5-8990-6BF1FAD00AB2");
        public static readonly Guid DataCorrectionNeedMoreInformation = new Guid("21480126-512F-4848-80D5-40BB2E7D4F3B");
        public static readonly Guid GenericConfirmation = new Guid("1EE1D7A3-E95A-438A-BDF3-1B769BB290EF");
        public static readonly Guid SiteStudyIpExpiration = new Guid("E4899AE6-E1C3-478B-9BE9-1DC8B7D90005");
        public static readonly Guid CRAReconciliation = new Guid("0B6AC8AE-14B9-43F9-9AD6-6CF550923031");
        public static readonly Guid SubjectUnblinded = new Guid("35C35965-0DBD-4D3E-AE99-7024FBEF7605");
        public static readonly Guid SubjectUnblindedBlinded = new Guid("AA535E16-6782-4F83-B5F5-72DA767297A0");
        public static readonly Guid ScreenFail = new Guid("E23CDF88-C5D2-43D7-B574-73A7E597B008");
        public static readonly Guid SiteManagement = new Guid("FAA312D7-212A-4E73-A22D-77F606AEC0D5");
        public static readonly Guid EarlyTermination = new Guid("18901012-7DF9-4481-BF20-797BAEC63AFE");
        public static readonly Guid Screening = new Guid("8A658560-987A-4171-A6E5-BE1DD1269751");
        public static readonly Guid TreatmentVisit = new Guid("1330F707-E84C-40F4-9518-DAB4626990CA");
        public static readonly Guid StudyCompletion = new Guid("78266C1F-BC86-4DFC-A9F5-F93456DB30A0");
        public static readonly Guid SiteActivation = new Guid("352471B1-6D18-4470-8282-D90BD03C816F");
        public static readonly Guid OutdatedVisit = new Guid("112B1D12-19AC-40CE-A694-1D43724000AF");
        public static readonly Guid SubjectsCountWarning = new Guid("6553285E-E81F-43C8-98D7-A1E120E66A38");
        public static readonly Guid WebDiaryBackupCompleted = new Guid("23237D64-FFF3-475F-89D3-1848018C9DB9");
        public static readonly Guid SubjectHandheldWebBackup = new Guid("0BDED019-6F77-4F06-8617-3D396C2906DA");
        public static readonly Guid BulkSiteManagement = new Guid("DB3D18BA-E8A8-4736-8793-D15B47526A99");
    }
}