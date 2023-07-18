using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YPrime.Data.Study.Models.Models.Constants;

namespace YPrime.Data.Study.Models.Models
{
    [Table("ScreenReportDialog")]
    public class ScreenReportDialog : AuditModel
    {
        [Key] public string TranslationKey { get; set; }

        public string ButtonConfirmTranslationKey { get; set; }

        public string ButtonCancelTranslationKey { get; set; }

        public string TitleTranslationKey { get; set; }

        [DefaultValue("'" + ScreenReportDeviceTypes.Both + "'")] // this needs to be a sql expression. so it needs to be a sql string in the string
        public string DeviceType { get; set; }

        [DefaultValue("'" + ScreenReportDialogTypes.Both + "'")]
        public string DialogType { get; set; }

        [DefaultValue(0)]
        public bool IsSiteFacing { get; set; }
    }
}