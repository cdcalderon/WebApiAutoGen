using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YPrime.Core.BusinessLayer.Models
{
    public class AlarmModel : IConfigModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public int? AlarmHour { get; set; }
        public int? AlarmMinute { get; set; }
        public int? MinimumAlarmHour { get; set; }
        public int? MinimumAlarmMinute { get; set; }
        public int? MaximumAlarmHour { get; set; }
        public int? MaximumAlarmMinute { get; set; }
        public int? SeriesRetries { get; set; }
        public int? SeriesInterval { get; set; }
        public string SeriesStopTime { get; set; }
        public int RingInterval { get; set; }
        public Guid? NotifyBusinessRuleId { get; set; }
        public bool? BusinessRuleTrueFalseIndicator { get; set; }
    }
}