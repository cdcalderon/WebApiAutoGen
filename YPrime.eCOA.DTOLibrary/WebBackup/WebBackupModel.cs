namespace YPrime.eCOA.DTOLibrary.WebBackup
{
    public class WebBackupModel
    {
        public bool CanDoWebBackup { get; set; }

        public string TimeZone { get; set; }

        public string Title { get; set; }

        public string Url { get; set; }

        public string WebBackupError { get; set; }

        public string WebBackupInstruction { get; set; }

        public int IFrameWidth { get; set; }

        public int IFrameHeight { get; set; }
    }
}