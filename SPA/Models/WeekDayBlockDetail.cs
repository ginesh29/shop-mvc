using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPA.Models
{
    public class WeekDayBlockDetail
    {
        public long UserId { get; set; }
        public string weekDay { get; set; }
        public TimeSpan MaxTime { get; set; }
        public TimeSpan MinTime { get; set; }
        public string SlotDue { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string Image { get; set; }
        public string AccessToData { get; set;}
        public string FlowStatus { get; set; }
        public string insertAccess { get; set; }
        public string DeleteAccess { get; set; }
        public string UpdateAccess { get; set; }
    }
    public class WeekScheduleAdd
    {
        public string WeekDay { get; set; }
        public List<string> Time { get; set; } = new List<string>();
        public List<TimeSpan> Times { get; set; } = new List<TimeSpan>();
    }
    public class WeekSchedule
    {
        public string WeekDay { get; set; }
        public TimeSpan? starttime { get; set; }
        public TimeSpan? endTime { get; set; }
        public int? periodNumber { get; set; }
        public int flg { get; set; }
    }
}