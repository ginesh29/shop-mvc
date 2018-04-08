                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPA.Models
{
    public class vacation
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Status { get; set; }
        public long? UserId { get; set; }


    }
    public class Holiday
    {
        public long EditLeaveId { get; set; }
        public string HolidayName { get; set; }
        public string HolidayType { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
    public class HolidayList
    {
        public long LeaveId { get; set; }
        public string HolidayDesc { get; set; }
        public long? Schid { get; set; }
        public long? UserId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string CalendarName { get; set;}
        public DateTime? CreatedOn { get; set;}
        public string AccessToData { get; set; }
        public string insertAccess { get; set; }
        public string DeleteAccess { get; set; }
        public string UpdateAccess { get; set; }
        public string FlowStatus { get; set; }
    }
    public class TextDisplay
    {
        public string ItenName { get; set;}
        public string Group { get; set;}
        public long? SolutionId { get; set;}
        public string Catgdesc { get; set;}
        public long? CatgId { get; set;}
        public string AccessToData { get; set; }
        public string insertAccess { get; set; }
        public string DeleteAccess { get; set; }
        public string UpdateAccess { get; set; }
        public long? CatgTypeId { get; set;}
        public string SectionDesc { get; set;}
        public string FlowStatus { get; set; }
    }
    public class quickBlockSlot
    {
        public TimeSpan? StartTime { get; set;}
        public TimeSpan? EndTime { get; set; }
        public string SlotDue { get; set; }
        public string ViewStatus { get; set; }
    }
    public class QuickBlockSlotDetails
    {
        public string StartDateTime { get; set; } = DateTime.Now.ToString("yyyy/MM/dd HH:mm");
        public string startTime { get; set; } = "00:00";
        public int StartMin { get; set; }
        public int StartHour { get; set; }
        public int Endmin { get; set; }
        public int EndHour { get; set; }
        public string Stepping { get; set; } = "5";    
        public string EditStartTime { get; set;}
        public string EditEndTime { get; set;}    
        public string EndTime { get; set; } = "00:00";
        public int EndStartMin { get; set; }
        public int EndStartHour { get; set; }
    }
    public class QuickBlockDetails
    {
        public string BlockDatePicker { get; set; }
        public string BlockStartTime { get; set; }
        public string BlockEndTime { get; set; }
        public long BlockUserId { get; set; }
        public string ViewStatus { get; set; }
        public int? ReservationId { get; set;}
        public string ShortLeaveComment { get; set;}
    }
    public class EditQuickBlockDetails
    {
        public int? ReservationId { get; set;}
        public string BookingDate { get; set;}
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string ShortLeaveComment { get; set;}
    }
}                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     