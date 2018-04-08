$.ajaxSetup({
    statusCode: {
        401: function () {
            gotoLogin();
        }
    }
});
function gotoLogin() {
    window.location.href = "/Login/SignIn";
}
/*Remove Browser Autocomplete Scripts*/
$("form").attr('autocomplete', 'off');

/*Calander Scripts*/
var monthsList = new Array(12);
monthsList[0] = "January";
monthsList[1] = "February";
monthsList[2] = "March";
monthsList[3] = "April";
monthsList[4] = "May";
monthsList[5] = "June";
monthsList[6] = "July";
monthsList[7] = "August";
monthsList[8] = "September";
monthsList[9] = "October";
monthsList[10] = "November";
monthsList[11] = "December";
function GetMonthNames(Number) {
    var monthsList = new Array(12);
    monthsList[0] = "January";
    monthsList[1] = "February";
    monthsList[2] = "March";
    monthsList[3] = "April";
    monthsList[4] = "May";
    monthsList[5] = "June";
    monthsList[6] = "July";
    monthsList[7] = "August";
    monthsList[8] = "September";
    monthsList[9] = "October";
    monthsList[10] = "November";
    monthsList[11] = "December";
    return monthsList[Number];
}
function SetDtMt(n) {
    var not = parseInt(n);
    return not > 9 ? "" + not : "0" + not;
}
function GetWeekNames(Number) {
    var weekday = new Array(7);
    weekday[0] = "Sunday";
    weekday[1] = "Monday";
    weekday[2] = "Tuesday";
    weekday[3] = "Wednesday";
    weekday[4] = "Thursday";
    weekday[5] = "Friday";
    weekday[6] = "Saturday"
    return weekday[Number];
}
function changeLanguage() { return $("#ResLangIdDefault").val(); }
function GetBookingDetails(ReservationId) {
    $(".loader").show();
    var views = $("#DisplayDay:visible,#DisplayWeek:visible,#DisplayMonth:visible").attr("id").replace("Display", "");
    var viewsAll = $("#DisplayDay:visible .employee-detail-slider a.activebutton,#DisplayWeek:visible .employee-detail-slider a.activebutton,#DisplayMonth:visible .employee-detail-slider a.activebutton").attr("id");
    if (viewsAll != "All") {
        viewsAll = null;
    }
    $.post("/Reservation/DisplayCalbookingDetails?ReservationId=" + ReservationId + "&view=" + views + "&AllView=" + viewsAll, function (Bookingdata) {
        $("#WelcomeAlert").html("<h1 class=\"text-center margin-bottom30\"><span>CLICK-AND-GO!</span></h1><p class=\"title15 text-center margin-bottom30\">" + Bookingdata + "</p>");
        $('#welcome').modal("show");
        $(".loader").hide();
    });
}
function QuickBlockerPopup() {
    $("#btnQuickBlocker,#btnMonthQuickBlocker,#btnWeekQuickBlocker").click(function () {
        var ViewStatus = "Day";
        if ($(this).prop("id") == "btnQuickBlocker")
            ViewStatus = "Day";
        if ($(this).prop("id") == "btnWeekQuickBlocker")
            ViewStatus = "Week";
        if ($(this).prop("id") == "btnMonthQuickBlocker")
            ViewStatus = "Month";
        $(".loader").show();
        var UserId = $(this).attr("BlockUserId");
        $.post("/Reservation/QuickBlockPopup?UserId=" + UserId + "&Status=" + ViewStatus, function (BlockDetails) {
            $("#WelcomeAlert").html("<h1 class=\"text-center margin-bottom30\"><span>CLICK-AND-GO!</span></h1><p class=\"title15 text-center margin-bottom30\">" + BlockDetails + "</p>");
            $('#welcome').modal("show");
            $(".loader").hide();
        });
    });

}
function MonthChangeInWeek(Month, years) {
    var Language = changeLanguage();
    moment.lang(Language);
    var Months = parseInt(moment($.trim($(".WeekDatesDisp").html().split('-')[0]), 'MMM').format('M')) + Month - 1;
    var Year = parseInt($.trim($(".WeekDatesDisp").html().split('-')[1])) + years;
    var CurrentDates = new Date(Year, Months, 1);
    FindWeekSchedule(CurrentDates);
    var CurWeek = GetWeekNumber(CurrentDates);
    var LangaugeMonth = moment(CurrentDates.getFullYear() + "-" + SetDtMt((CurrentDates.getMonth() + 1)) + "-" + SetDtMt(CurrentDates.getDate())).lang(Language).format('MMMM');
    $(".WeekNumbersDisplay span").html(" " + CurWeek);
    $(".WeekDatesDisp").html(LangaugeMonth + " - " + CurrentDates.getFullYear());
    GoWeekCalender();

}
function GetAllDefaultForMonth(AllEmployee_Date) {
    var LanguageUsed = changeLanguage();
    var CurrentDate = new Date();
    if (AllEmployee_Date != null && AllEmployee_Date != "" && AllEmployee_Date != undefined) {
        CurrentDate.setDate(parseInt(AllEmployee_Date.split('-')[0]));
        CurrentDate.setMonth(parseInt(AllEmployee_Date.split('-')[1]));
        CurrentDate.setFullYear(parseInt(AllEmployee_Date.split('-')[2]));
    }
    var LangaugeMonth = moment(CurrentDate.getFullYear() + "-" + SetDtMt((CurrentDate.getMonth() + 1)) + "-" + SetDtMt(CurrentDate.getDate())).lang(LanguageUsed).format('MMMM');
    //To Set Current Month and Year
    $(".Header-FullCalander").html(LangaugeMonth + " - " + CurrentDate.getFullYear());
    //To Get Start Date To set Dates
    var startdate = new Date(CurrentDate.getFullYear(), SetDtMt(CurrentDate.getMonth()), 1);
    //Day of StartDate
    var startwithdate = startdate.getDay();
    //No. of possible dates in month
    var MaxDays = daysInMonth(SetDtMt(CurrentDate.getMonth()) + 1, CurrentDate.getFullYear());
    var CountingData = 0;
    var CountContinue = 1;
    $("#dayCalc tr th:not(.DayTitle)").html("");
    //To fill Dates
    $.each($("#dayCalc tr th"), function () {
        if (!$(this).hasClass("DayTitle")) {
            if ($(this).prop("id") == "dates_" + startwithdate) {
                CountingData = 1;
            }
            if (CountingData == 1 && CountContinue <= MaxDays) {
                $(this).html(CountContinue);
                CountContinue = CountContinue + 1;
            }
        }

    });
}
function GetBooking(Date, UserId, View, AllView) {
    var EmpUserId = parseInt(UserId);
    $.ajax({
        url: "/Reservation/AddBasicReservation?EmpUserId=" + EmpUserId + "&Date=" + Date.split(' ')[0] + "&Time=" + Date.split(' ')[1] + "&View=" + View + "&AllView=" + AllView,
        cache: false,
        success: function (ReservationId) {
            window.location.href = "/Reservation/CustomerList?ReservationId=" + ReservationId;
        },
        error: function (request, status, error) { }
    });
}
function CalBookOrder(Id) {
    $(".loader").show();
    var Comments = $("#Comments").val();
    $.post("/Reservation/CalBookOrder?ReservationId=" + Id + "&Comments=" + Comments, function (data) {
        window.location.href = "/Reservation/Reservation#calendarmonth";
    });
}
function AddQuickBlock() {
    var Addvalidate = $("#frmQuickBlocker").validate().form();
    if (Addvalidate != false) {
        var formData = new FormData($("#frmQuickBlocker")[0]);
        $.ajax({
            url: $("#frmQuickBlocker").attr("action"),
            beforeSend: function () {
                $(".loader").show();
                $('#welcome').modal("hide");
            },
            type: 'POST',
            data: formData,
            async: true,
            success: function (data) {
                $(".loader").hide();
                if (window.location.toString().indexOf('#') != -1) {
                    var ab = window.location.hash.substr(1);
                    if (ab == "calendarmonth") { location.reload(); }
                    else {
                        window.location.hash = 'calendarmonth';
                        window.location.reload();
                    }
                }
                else {
                    window.location.hash = '#calendarmonth';
                    window.location.reload();
                }
            },
            cache: false,
            contentType: false,
            processData: false
        });
        return false;
    }
    else { $('.loader').hide(); }
}
function ShowTodayCalData() {
    $(".loader").show();
    GetAllDefaultForMonth();
    GetAllDefaultsForWeek();
    DailyDefault();
    GoWeekCalender();
    GoAdd();
    //GoDailyCalender();
}
function RedirectTootherpage(Emp_UserId, Date) {
    GetAllDefaultForMonth(Date);
    $("#Employeesbtn a").removeClass("activebutton");
    $("#Employeesbtn #" + Emp_UserId).addClass("activebutton");
    $("#UserNamesForMonth").html($("#Employeesbtn .activebutton span").html());
    GoAdd();
    GetAllDefaultsForWeek(Date);
    $("#EmployeesWeekbtn a").removeClass("activebutton");
    $("#EmployeesWeekbtn #" + Emp_UserId).addClass("activebutton");
    $("#UserNamesForWeek").html($("#EmployeesWeekbtn .activebutton span").html());
    GoWeekCalender();
    DailyDefault(Date);
    $("#EmployeesDaybtn a").removeClass("activebutton");
    $("#EmployeesDaybtn #" + Emp_UserId).addClass("activebutton");
    $("#UserNames").html($("#EmployeesDaybtn .activebutton span").html());
    GoDailyCalender();
}
function ShowDataCala(UserId) {
    var MonthName = parseInt(moment($.trim($(".Header-FullCalander").html().split('-')[0]), 'MMM').format('M')) - 1;
    var YearName = $.trim($(".Header-FullCalander").html().split('-')[1]);
    var AllEmployee_Date = 1 + "-" + MonthName + "-" + YearName;
    if ($("#Employeesbtn .activebutton").prop("id") != UserId) {
        $("#EmplyeeDetails").html("");
        $("#Employeesbtn a").removeClass("activebutton");
        $("#EmployeesWeekbtn a").removeClass("activebutton");
        $("#EmployeesDaybtn a").removeClass("activebutton");
        $("#Employeesbtn #" + UserId).addClass("activebutton");
        $("#EmployeesWeekbtn #" + UserId).addClass("activebutton");
        $("#EmployeesDaybtn #" + UserId).addClass("activebutton");
        $("#UserNamesForMonth").html($("#Employeesbtn .activebutton span").html());
        $("#UserNamesForWeek").html($("#EmployeesWeekbtn .activebutton span").html());
        $("#UserNames").html($("#EmployeesDaybtn .activebutton span").html());
        GetAllDefaultForMonth(AllEmployee_Date);
        GoWeekCalender();
        GoAdd();
        GoDailyCalender();
    }
}
function GetAllDefaultsForWeek(AllEmployee_Date) {
    var CurrentDates = new Date();
    //Testing
    if (AllEmployee_Date != null && AllEmployee_Date != "" && AllEmployee_Date != undefined) {
        CurrentDates.setDate(parseInt(AllEmployee_Date.split('-')[0]));
        CurrentDates.setMonth(parseInt(AllEmployee_Date.split('-')[1]));
        CurrentDates.setFullYear(parseInt(AllEmployee_Date.split('-')[2]));
    }
    FindWeekSchedule(CurrentDates);
    var LangaugeMonth = moment(CurrentDates.getFullYear() + "-" + SetDtMt((CurrentDates.getMonth() + 1)) + "-" + SetDtMt(CurrentDates.getDate())).lang(changeLanguage()).format('MMMM');
    var CurWeek = GetWeekNumber(CurrentDates);
    $(".WeekNumbersDisplay span").html(" " + CurWeek);
    $(".WeekDatesDisp").html(LangaugeMonth + " - " + CurrentDates.getFullYear());
}
function BlockBookingCalander(MonthName, YearName, Blocking, status) {
    var ArrayRow = new Array();
    var QArrayRow = new Array();
    var EachLoop = "";
    var NewTagtxt = "";
    var CompareClass = "";
    var Unuti = "";
    //if (status == "Daily") {
    //    NewTagtxt = "";
    //    EachLoop = "DayCalBodyTag";
    //}
    if (status == "Weekly") {
        NewTagtxt = "Weeks-Dates";
        EachLoop = "WeekCalBodyTag";
        CompareClass = "DateTitle";
        Unuti = "UnoccWeek";

    }
    if (status == "Monthly") {
        EachLoop = "BodyTag";
        NewTagtxt = "dayCalc";
        CompareClass = "DayTitle";
        Unuti = "UnoccMonth";
    }
    var Slot = $("#" + EachLoop).find('tr').first().prop("class");
    var Lnth = $("#" + EachLoop).find('tr').length;
    var BlockingCheck = Blocking.DataSlot;
    var QuickBlocking = Blocking.QuickBlock;
    var Daysga = Blocking.WeekdaysBlock;

    var Holiday = Blocking.employeelive;
    var CountTr = 0;
    var ReplacingTxt = new Array();
    var oldReplaceTxt = new Array();
    var ReplacingCnt = 0;
    var QReplacingTxt = new Array();
    var QoldReplaceTxt = new Array();
    var QReplacingCnt = 0;
    $.ajax({
        url: "/Reservation/getCalendarConverted",
        cache: false,
        dataType: "json",
        success: function (jsonOutput) {
            $.each($("#" + EachLoop + " tr"), function () {
                var count = 1;
                $.each($(this).children("td"), function () {

                    var GetStream = $(this);
                    var NewTag = $("#" + NewTagtxt + " tr th:nth-child(" + count + ")");
                    if (!(NewTag.hasClass(CompareClass))) {
                        //Get the date from Full Date
                        var GetDates = ($.trim(NewTag.html())).split('-')[0];
                        if (GetDates != "") {
                            //Get the current Hour
                            var Day = GetDates;
                            var Hour = GetStream.prop("id").split(':')[0];
                            //Get the current Minute
                            var Minutes = GetStream.prop("id").split(':')[1];
                            //Get the Date
                            var CurrentDates = new Date(YearName, MonthName, Day, Hour, Minutes, 0, 0);
                            //Not Available Days Block

                            var Daysga = Blocking.WeekdaysBlock;
                            if (Daysga.indexOf(GetWeekNames(CurrentDates.getDay())) < 0) {
                                //$("#" + EachLoop).find(this).addClass("NotAvailable").html("Not <br/>Availble");
                                $("#" + EachLoop).find(this).addClass("NotAvailable").html(jsonOutput[2].Value);
                            }
                            //This hour of Day is not
                            var AllDay = Blocking.NotAvailableDay;
                            var AllTime = Blocking.NotAvailableTime;
                            if ($.inArray(GetWeekNames(CurrentDates.getDay()), AllDay) !== -1) {

                                var NotPos = $.inArray(GetWeekNames(CurrentDates.getDay()), AllDay);
                                var Hourstart = AllTime[NotPos].split(' ')[0].split(':')[0];
                                var Minutestart = AllTime[NotPos].split(' ')[0].split(':')[1];
                                var HoursEnd = AllTime[NotPos].split(' ')[1].split(':')[0];
                                var MinuteEnd = AllTime[NotPos].split(' ')[1].split(':')[1];
                                var NoStart = new Date(YearName, MonthName, Day, Hourstart, Minutestart, 0, 0);
                                var NoEnd = new Date(YearName, MonthName, Day, HoursEnd, MinuteEnd, 0, 0);
                                if (CurrentDates >= NoStart && CurrentDates < NoEnd)
                                { }
                                else { $(this).addClass("NotAvailable").html(jsonOutput[2].Value); }
                            }
                            //HolidayBlock
                            var Holiday = Blocking.employeelive;
                            for (i = 0; i <= Holiday.length - 1; i++) {
                                if (Holiday[i] != "") {
                                    var ConsiderHoliday0 = parseInt(Holiday[i].split(' ')[0]);
                                    var ConsiderHoliday1 = parseInt(Holiday[i].split(' ')[1]);
                                    if (ConsiderHoliday0 >= parseInt(GetDates) || ConsiderHoliday0 <= parseInt(GetDates)) {
                                        var BlockesTxt = Holiday[i].replace('~', '').split(' ');
                                        var Dtconsider = new Date(YearName, MonthName, BlockesTxt[0], Hour, Minutes);
                                        var DtAfter = new Date(YearName, MonthName, BlockesTxt[1], Hour, Minutes);
                                        // && !GetStream.hasClass("NotAvailable")
                                        if (Dtconsider <= CurrentDates && DtAfter >= CurrentDates) {
                                            $("#" + EachLoop).find(this).addClass("holidayAdd").html(jsonOutput[3].Value);
                                            //GetStream.addClass("holidayAdd");
                                            //GetStream.html("Holiday");
                                        }
                                    }
                                }
                            }
                            //Block Bookings
                            for (i = 0; i <= BlockingCheck.length - 1; i++) {
                                if (BlockingCheck[i] != "") {
                                    var Consider = BlockingCheck[i].split(' ')[0];
                                    if (parseInt(Consider) == parseInt(GetDates)) {
                                        var BlockesTxt = BlockingCheck[i].replace('~', '').split(' ');
                                        var Dtconsider = new Date(YearName, MonthName, BlockesTxt[0], BlockesTxt[1].split(':')[0], BlockesTxt[1].split(':')[1], 0, 0);
                                        var DtAfter = new Date(YearName, MonthName, BlockesTxt[0], BlockesTxt[2].split(':')[0], BlockesTxt[2].split(':')[1], 0, 0);
                                        var Status = BlockingCheck[i].split(" ")[3];
                                        var ResId = BlockingCheck[i].split(" ")[5];
                                        var CurrentDatesTill = new Date(YearName, MonthName, SetDtMt(CurrentDates.getDate()), SetDtMt(CurrentDates.getHours()), SetDtMt(CurrentDates.getMinutes()) + SetDtMt(parseInt(Slot)), 0, 0);
                                        var TempDate = "";

                                        //if (CurrentDates < Dtconsider && $("#" + EachLoop).hasClass("NotAvailable")) {
                                        //    if (CurrentDatesTill > Dtconsider) {
                                        //        TempDate = "Done";
                                        //    }
                                        //}
                                        if (((CurrentDates >= Dtconsider && CurrentDates < DtAfter) || TempDate == "Done") && !GetStream.hasClass("holidayAdd") && !GetStream.hasClass("NotAvailable")) {

                                            //if (CurrentDates.getDate() == Dtconsider.getDate() && CurrentDates.getMonth() == Dtconsider.getMonth() && CurrentDates.getFullYear() == Dtconsider.getFullYear() && (Dtconsider.getHours() >= CurrentDates.getHours() && Dtconsider.getHours() < CurrentDatesTill.getHours()) && (Dtconsider.getMinutes() >= CurrentDates.getMinutes() && Dtconsider.getMinutes() < CurrentDatesTill.getMinutes())) {
                                            if (Dtconsider >= CurrentDates && Dtconsider < CurrentDatesTill) {
                                                //if (TempDate == "Done") {

                                                //    var Monthpls = parseInt(MonthName) + 1;
                                                //    ReplacingTxt[ReplacingCnt] = SetDtMt(CurrentDates.getDate()) + "-" + Monthpls + "-" + YearName + " " + SetDtMt(CurrentDates.getHours()) + ":" + SetDtMt(CurrentDates.getMinutes());
                                                //    oldReplaceTxt[ReplacingCnt] = SetDtMt(Dtconsider.getDate()) + "-" + Monthpls + "-" + YearName + " " + SetDtMt(Dtconsider.getHours()) + ":" + SetDtMt(Dtconsider.getMinutes());
                                                //    ReplacingCnt = ReplacingCnt + 1;
                                                //    TempDate = "";
                                                //}
                                                if (Status == "A") {
                                                    $("#" + EachLoop).find(this).addClass("BookedSlotes").addClass("Id-" + ResId).html(jsonOutput[0].Value);
                                                    ArrayRow[i] = 1;
                                                }
                                                else if (Status == "C") {
                                                    $("#" + EachLoop).find(this).addClass("ApCloseBlock").addClass("Id-" + ResId).html(jsonOutput[5].Value);
                                                    ArrayRow[i] = 1;
                                                }
                                                else {
                                                    $("#" + EachLoop).find(this).addClass("Temporary").addClass("Id-" + ResId).html(jsonOutput[4].Value);
                                                    ArrayRow[i] = 1;
                                                }

                                            }
                                            else {

                                                if (Status == "A") {
                                                    $("#" + EachLoop).find(this).addClass("BookedSlotes").html(jsonOutput[0].Value).hide();
                                                    if (ArrayRow[i] == undefined || ArrayRow[i] == 0) { ArrayRow[i] = 2 }
                                                    else { ArrayRow[i] = ArrayRow[i] + 1; }
                                                }
                                                else if (Status == "C") {
                                                    $("#" + EachLoop).find(this).addClass("ApCloseBlock").html(jsonOutput[5].Value).hide();
                                                    if (ArrayRow[i] == undefined || ArrayRow[i] == 0) { ArrayRow[i] = 2 }
                                                    else { ArrayRow[i] = ArrayRow[i] + 1; }
                                                }
                                                else {
                                                    $("#" + EachLoop).find(this).addClass("Temporary").html(jsonOutput[4].Value).hide();
                                                    if (ArrayRow[i] == undefined || ArrayRow[i] == 0) { ArrayRow[i] = 2 }
                                                    else { ArrayRow[i] = ArrayRow[i] + 1; }
                                                }

                                            }
                                        }
                                    }
                                }
                            }

                            for (i = 0; i <= QuickBlocking.length - 1; i++) {
                                if (QuickBlocking[i] != "" && !GetStream.hasClass("NotAvailable")) {
                                    var Consider = QuickBlocking[i].split(' ')[0];
                                    if (parseInt(Consider) == parseInt(GetDates)) {
                                        var BlockesTxt = QuickBlocking[i].replace('~', '').split(' ');
                                        var Dtconsider = new Date(YearName, MonthName, BlockesTxt[0], BlockesTxt[1].split(':')[0], BlockesTxt[1].split(':')[1], 0, 0);
                                        var DtAfter = new Date(YearName, MonthName, BlockesTxt[0], BlockesTxt[2].split(':')[0], BlockesTxt[2].split(':')[1], 0, 0);
                                        var Status = QuickBlocking[i].split(" ")[3];
                                        var UserId = $.trim(QuickBlocking[i].split(" ")[5]);
                                        var ResId = QuickBlocking[i].split(" ")[4];
                                        var BookingDate = QuickBlocking[i].split(" ")[6];
                                        var Comment = QuickBlocking[i].split(" ")[7] != undefined && QuickBlocking[i].split(" ")[7] != "" ? (QuickBlocking[i].split(" ")[7]).toString().replace(/[*]/g, ' ') : jsonOutput[7].Value;
                                        var CurrentDatesTill = new Date(YearName, MonthName, SetDtMt(CurrentDates.getDate()), SetDtMt(CurrentDates.getHours()), SetDtMt(CurrentDates.getMinutes()) + SetDtMt(parseInt(Slot)), 0, 0);
                                        var TempDate = "";
                                        //if (CurrentDates >= Dtconsider && CurrentDates < DtAfter) {

                                        //    if (Status == "DA") {
                                        //        var Html = "<div class='Quick_text'>" + jsonOutput[7].Value + "</div> <div class='Quick_Edit'>" +
                                        //                   "<i class='fa fa-pencil-square-o margin-right5' ResId=" + ResId + " UserId=" + UserId + " aria-hidden='true'></i><i class='fa fa-trash' ResId=" + ResId + " BookingDate=" + BookingDate + " aria-hidden='true'></i>" +
                                        //                   "</div>";
                                        //        $(this).addClass("Quick_Holiday").addClass("Id-" + ResId).html(Html);

                                        //    }
                                        //}
                                        if (((CurrentDates >= Dtconsider && CurrentDates < DtAfter) || TempDate == "Done") && !GetStream.hasClass("holidayAdd") && !GetStream.hasClass("NotAvailable")) {
                                            var QuickHtml = "<div class='Quick_text'>" + Comment + "</div> <div class='Quick_Edit'>" +
                                                          "<div><i class='fa fa-pencil-square-o margin-right5' ResId=" + ResId + " UserId=" + UserId + " aria-hidden='true'></i><i class='fa fa-trash' ResId=" + ResId + " BookingDate=" + BookingDate + " aria-hidden='true'></i></div>" +
                                                          "</div>";
                                            //if (CurrentDates.getDate() == Dtconsider.getDate() && CurrentDates.getMonth() == Dtconsider.getMonth() && CurrentDates.getFullYear() == Dtconsider.getFullYear() && (Dtconsider.getHours() >= CurrentDates.getHours() && Dtconsider.getHours() < CurrentDatesTill.getHours()) && (Dtconsider.getMinutes() >= CurrentDates.getMinutes() && Dtconsider.getMinutes() < CurrentDatesTill.getMinutes())) {
                                            if (Dtconsider >= CurrentDates && Dtconsider < CurrentDatesTill) {
                                                //if (TempDate == "Done") {

                                                //    var Monthpls = parseInt(MonthName) + 1;
                                                //    ReplacingTxt[ReplacingCnt] = SetDtMt(CurrentDates.getDate()) + "-" + Monthpls + "-" + YearName + " " + SetDtMt(CurrentDates.getHours()) + ":" + SetDtMt(CurrentDates.getMinutes());
                                                //    oldReplaceTxt[ReplacingCnt] = SetDtMt(Dtconsider.getDate()) + "-" + Monthpls + "-" + YearName + " " + SetDtMt(Dtconsider.getHours()) + ":" + SetDtMt(Dtconsider.getMinutes());
                                                //    ReplacingCnt = ReplacingCnt + 1;
                                                //    TempDate = "";
                                                //}
                                                if (Status == "DA") {
                                                    $("#" + EachLoop).find(this).addClass("Quick_Holiday").addClass("Id-" + ResId).html(QuickHtml);
                                                    QArrayRow[i] = 1;
                                                }
                                            }
                                            else {
                                                if (Status == "DA") {
                                                    $("#" + EachLoop).find(this).addClass("Quick_Holiday").html(QuickHtml).hide();
                                                    if (QArrayRow[i] == undefined || QArrayRow[i] == 0) { QArrayRow[i] = 2 }
                                                    else { QArrayRow[i] = QArrayRow[i] + 1; }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            var DisplayMonth = parseInt(MonthName) + 1;
                            $("#" + EachLoop).find(this).prop("id", GetDates + "-" + DisplayMonth + "-" + YearName + " " + Hour + ":" + Minutes);
                        }
                        else {
                            $("#" + EachLoop).find(this).addClass("UnknownP").prop("id", "");
                        }
                        count = count + 1;
                    }
                    else {
                        count = count + 1;
                    }
                    if (!$("#" + EachLoop).find(this).hasClass("UnknownP") && !$("#" + EachLoop).find(this).hasClass("Quick_Holiday") && !$("#" + EachLoop).find(this).hasClass("BookedSlotes") && !$("#" + EachLoop).find(this).hasClass("ApCloseBlock") && !$("#" + EachLoop).find(this).hasClass("NotAvailable") && !$("#" + EachLoop).find(this).hasClass("holidayAdd") && $("#" + EachLoop).find(this).prop("colspan") != 2 && !$("#" + EachLoop).find(this).hasClass("Temporary")) {
                        $("#" + EachLoop).find(this).addClass("Available").html(jsonOutput[1].Value);
                    }
                });
                //CountTr = CountTr + 1;
            });
            for (i = 0; i <= BlockingCheck.length - 1; i++) {
                if (BlockingCheck[i] != "") {
                    if (ArrayRow[i] != undefined) {
                        var DisplayMonth = parseInt(MonthName) + 1;
                        var DatesFun = BlockingCheck[i].split(" ")[0];
                        var HoursTemp = BlockingCheck[i].split(" ")[1].split(':')[0];
                        var MinutesTemp = BlockingCheck[i].split(" ")[1].split(':')[1];
                        var Name = (BlockingCheck[i].split(" ")[4]).toString().replace(/[*]/g, ' ');

                        var GoToElement = DatesFun + "-" + DisplayMonth + "-" + YearName + " " + HoursTemp + ":" + MinutesTemp;
                        if ($.inArray(GoToElement, oldReplaceTxt) !== -1) {
                            var PositionPlace = $.inArray(GoToElement, oldReplaceTxt);
                            GoToElement = ReplacingTxt[PositionPlace];
                        }
                        var RowspanValue = "" + ArrayRow[i];
                        if (!$("#" + EachLoop).find("[id='" + GoToElement + "']").hasClass("holidayAdd") && !$("#" + EachLoop).find("[id='" + GoToElement + "']").hasClass("NotAvailable") && !$("#" + EachLoop).find("[id='" + GoToElement + "']").hasClass("UnknownP")) {
                            $("#" + EachLoop).find("[id='" + GoToElement + "']").prop("rowspan", RowspanValue).html(Name);
                            //.addClass("Border-Book")
                        }
                    }
                }
            }
            for (i = 0; i <= QuickBlocking.length - 1; i++) {
                if (QuickBlocking[i] != "") {
                    if (QArrayRow[i] != undefined) {
                        var DisplayMonth = parseInt(MonthName) + 1;
                        var DatesFun = QuickBlocking[i].split(" ")[0];
                        var HoursTemp = QuickBlocking[i].split(" ")[1].split(':')[0];
                        var MinutesTemp = QuickBlocking[i].split(" ")[1].split(':')[1];
                        var UserId = $.trim(QuickBlocking[i].split(" ")[5]);
                        var ResId = QuickBlocking[i].split(" ")[4];
                        var BookingDate = QuickBlocking[i].split(" ")[6];
                        var Comment = QuickBlocking[i].split(" ")[7] != undefined && QuickBlocking[i].split(" ")[7] != "" ? (QuickBlocking[i].split(" ")[7]).toString().replace(/[*]/g, ' ') : jsonOutput[7].Value;
                        var QuickHtml = "<div class='Quick_text'>" + Comment + "</div> <div class='Quick_Edit'>" +
                                        "<div><i class='fa fa-pencil-square-o margin-right5' ResId=" + ResId + " UserId=" + UserId + " aria-hidden='true'></i><i class='fa fa-trash' ResId=" + ResId + " BookingDate=" + BookingDate + " aria-hidden='true'></i></div>" +
                                        "</div>";
                        var GoToElement = DatesFun + "-" + DisplayMonth + "-" + YearName + " " + HoursTemp + ":" + MinutesTemp;
                        if ($.inArray(GoToElement, QoldReplaceTxt) !== -1) {
                            var PositionPlace = $.inArray(GoToElement, QoldReplaceTxt);
                            GoToElement = QReplacingTxt[PositionPlace];
                        }
                        var RowspanValue = "" + QArrayRow[i];
                        if (!$("#" + EachLoop).find("[id='" + GoToElement + "']").hasClass("holidayAdd") && !$("#" + EachLoop).find("[id='" + GoToElement + "']").hasClass("NotAvailable") && !$("#" + EachLoop).find("[id='" + GoToElement + "']").hasClass("UnknownP")) {
                            $("#" + EachLoop).find("[id='" + GoToElement + "']").prop("rowspan", RowspanValue).html(QuickHtml);
                            //.addClass("Border-Book")
                        }
                    }
                }
            }
            var AvailCnt = $("#" + EachLoop).find(".Available").length;
            var TotalCnt = $('#' + EachLoop + ' tr td:not([colspan=2])').not("Quick_Holiday").not(".NotAvailable").not(".holidayAdd").not(".UnknownP").length;
            var Unutilized = 0;
            if (AvailCnt != 0 && TotalCnt != 0) {
                Unutilized = parseInt((AvailCnt / TotalCnt) * 100);
            }
            $("#" + Unuti + " span").html(Unutilized + "%");
        },
        complete: function () {
            $(".loader").hide();
        },
        error: function (request, status, error) { }
    });

}
function GoAllemployeeDetails() {
    $(".loader").show();
    var LanguageUsed = changeLanguage();
    //All Variable
    var ArrayRow = new Array();
    var QArrayRow = new Array();
    var UserId = $("#EmployeesDaybtn .active").prop("id");
    var MonthName = parseInt(moment($.trim($(".Header-FullCalanderDaily").html().split('-')[0]), 'MMM').format('M')) - 1;
    var YearName = $.trim($(".Header-FullCalanderDaily").html().split('-')[1]);
    var Day = ($.trim($(".DateLatestDaily").html())).split(' ')[1].split("-")[0];
    var statusAllEmplyee = "AllEmployee";
    var FullDate = Day + "-" + (parseInt(MonthName) + 1) + "-" + YearName;
    var EngMonth = moment($.trim($(".Header-FullCalanderDaily").html().split('-')[0]), 'MMM', LanguageUsed).lang('en').format("MMMM");
    $.ajax({
        url: "/Reservation/getCalendarConverted",
        cache: false,
        dataType: "json",
        success: function (JsonTranslation) {
            $.post("/Reservation/EmployeeDetails", function (Emplyeedata) {
                $("#EmplyeeDetails").html(Emplyeedata);
                $.post("/Reservation/BlockAllEmployeeDayDetail?GetDate=" + FullDate, function (data) {
                    $("#DayCalBodyTag").html(data);
                    $.ajax({
                        url: "/Reservation/GetAllBookedData?UserId=0&Monthname=" + EngMonth + "&yearname=" + YearName + "&Day=" + ($.trim($(".DateLatestDaily").html())).split(' ')[1].split("-")[0] + "&Status=" + statusAllEmplyee,
                        type: 'POST',
                        dataType: "json",
                        async: true,
                        success: function (Blocking) {
                            $("#DailyRevenue span").html(Blocking.TotalRev.replace(/[*]/g, ' '));
                            //Initialization
                            var Lnth = $("#DayCalBodyTag").find('tr').length;
                            var BlockingCheck = Blocking.DataSlot;
                            var QuickBlocking = Blocking.QuickBlock;
                            var Daysga = Blocking.WeekdaysBlock;
                            var CountTr = 0;
                            var Slot = $("#DayCalBodyTag").find('tr').first().prop("class");
                            var ReplacingTxt = new Array();
                            var oldReplaceTxt = new Array();
                            var ReplacingCnt = 0;
                            var QReplacingTxt = new Array();
                            var QoldReplaceTxt = new Array();
                            var QReplacingCnt = 0;
                            var TimeSlotSet = Blocking.TimeSlotAvailable;
                            var TkslotDate = new Date();
                            var Tkscnt = 0;
                            var ArrayEmp = new Array();
                            var ArrayEmpCount = new Array();
                            var ArrayEmpDate = new Array();
                            $.each($("#DayCalBodyTag tr"), function () {
                                var count = 1;
                                $.each($(this).children("td:not([colspan=2])"), function () {
                                    var GetStream = $(this);
                                    var GetDates = ($.trim($(".DateLatestDaily").html())).split(' ')[1].split("-")[0];
                                    if (GetDates != "") {
                                        //Get the current Hour
                                        var Day = GetDates;
                                        var Hour = $(this).prop("id").split(':')[0];
                                        //Get the current Minute
                                        var Minutes = $(this).prop("id").split(':')[1].split('~')[0];
                                        //Get the Date
                                        var CurrentDates = new Date(YearName, MonthName, Day, Hour, Minutes, 0, 0);

                                        //UserId
                                        var UId = $.trim($(this).prop("id").split('~')[1]);

                                        //Not Available Days Block
                                        if (Daysga.indexOf(GetWeekNames(CurrentDates.getDay()) + " " + UId) < 0) {
                                            GetStream.addClass("NotAvailable").html(JsonTranslation[2].Value);
                                            //This hour of Day is not
                                        }
                                        //Not Available Time Related
                                        var AllDay = Blocking.NotAvailableDay;
                                        var AllTime = Blocking.NotAvailableTime;
                                        if ($.inArray((GetWeekNames(CurrentDates.getDay()) + " " + UId), AllDay) !== -1) {
                                            var NotPos = $.inArray((GetWeekNames(CurrentDates.getDay()) + " " + UId), AllDay);
                                            var Hourstart = AllTime[NotPos].split(' ')[0].split(':')[0];
                                            var Minutestart = AllTime[NotPos].split(' ')[0].split(':')[1];
                                            var HoursEnd = AllTime[NotPos].split(' ')[1].split(':')[0];
                                            var MinuteEnd = AllTime[NotPos].split(' ')[1].split(':')[1];
                                            var NoStart = new Date(YearName, MonthName, Day, Hourstart, Minutestart, 0, 0);
                                            var NoEnd = new Date(YearName, MonthName, Day, HoursEnd, MinuteEnd, 0, 0);
                                            if (CurrentDates >= NoStart && CurrentDates < NoEnd)
                                            { }
                                            else { GetStream.addClass("NotAvailable").html(JsonTranslation[2].Value); }
                                        }
                                        //HolidayBlock
                                        var Holiday = Blocking.employeelive;
                                        for (i = 0; i <= Holiday.length - 1; i++) {
                                            if (Holiday[i] != "" && !GetStream.hasClass("NotAvailable")) {
                                                var ConsiderHoliday0 = parseInt(Holiday[i].split(' ')[0]);
                                                var ConsiderHoliday1 = parseInt(Holiday[i].split(' ')[1]);
                                                if ((ConsiderHoliday0 >= parseInt(GetDates) || ConsiderHoliday0 <= parseInt(GetDates)) && (($.trim(Holiday[i].split(' ')[2]) == $.trim($(this).prop("id").split('~')[1])) || ($.trim(Holiday[i].split(' ')[3]) == "1"))) {
                                                    var BlockesTxt = Holiday[i].replace('~', '').split(' ');
                                                    var Dtconsider = new Date(YearName, MonthName, BlockesTxt[0], Hour, Minutes);
                                                    var DtAfter = new Date(YearName, MonthName, BlockesTxt[1], Hour, Minutes);
                                                    //&& !GetStream.hasClass("NotAvailable")
                                                    if (Dtconsider <= CurrentDates && DtAfter >= CurrentDates) {
                                                        GetStream.addClass("holidayAdd").html(JsonTranslation[3].Value);
                                                    }
                                                }
                                            }
                                        }
                                        //Block Bookings
                                        if (!GetStream.hasClass("NotAvailable")) {
                                            for (i = 0; i <= BlockingCheck.length - 1; i++) {
                                                if (BlockingCheck[i] != "" && !GetStream.hasClass("NotAvailable") && !GetStream.hasClass("holidayAdd")) {
                                                    var Consider = BlockingCheck[i].split(' ')[0];
                                                    if (parseInt(Consider) == parseInt(GetDates)) {
                                                        var BlockesTxt = BlockingCheck[i].replace('~', '').split(' ');
                                                        var Dtconsider = new Date(YearName, MonthName, BlockesTxt[0], BlockesTxt[1].split(':')[0], BlockesTxt[1].split(':')[1], 0, 0);
                                                        var DtAfter = new Date(YearName, MonthName, BlockesTxt[0], BlockesTxt[2].split(':')[0], BlockesTxt[2].split(':')[1], 0, 0);
                                                        var Status = BlockingCheck[i].split(" ")[3];
                                                        var UserId = $.trim(BlockingCheck[i].split(" ")[6]);
                                                        var ResId = BlockingCheck[i].split(" ")[5];
                                                        var CurrentDatesTill = new Date(YearName, MonthName, SetDtMt(CurrentDates.getDate()), SetDtMt(CurrentDates.getHours()), SetDtMt(CurrentDates.getMinutes()) + SetDtMt(parseInt(Slot)), 0, 0);
                                                        var TempDate = "";
                                                        //if (CurrentDates < Dtconsider) {
                                                        //    if (CurrentDatesTill > Dtconsider) {
                                                        //        TempDate = "Done";
                                                        //    }
                                                        //}
                                                        if (((CurrentDates >= Dtconsider && CurrentDates < DtAfter) || TempDate == "Done") && !GetStream.hasClass("holidayAdd") && UserId == $.trim($(this).prop("id").split('~')[1])) {
                                                            if (Dtconsider >= CurrentDates && Dtconsider < CurrentDatesTill) {
                                                                if (TempDate == "Done") {
                                                                    var Monthpls = parseInt(MonthName) + 1;
                                                                    ReplacingTxt[ReplacingCnt] = SetDtMt(CurrentDates.getDate()) + "-" + Monthpls + "-" + YearName + " " + SetDtMt(CurrentDates.getHours()) + ":" + SetDtMt(CurrentDates.getMinutes());
                                                                    oldReplaceTxt[ReplacingCnt] = SetDtMt(Dtconsider.getDate()) + "-" + Monthpls + "-" + YearName + " " + SetDtMt(Dtconsider.getHours()) + ":" + SetDtMt(Dtconsider.getMinutes());
                                                                    ReplacingCnt = ReplacingCnt + 1;
                                                                    TempDate = "";
                                                                }
                                                                if (Status == "A") {
                                                                    $(this).addClass("BookedSlotes").addClass("Id-" + ResId).html(JsonTranslation[0].Value);
                                                                    ArrayRow[i] = 1;
                                                                }
                                                                else if (Status == "C") {
                                                                    $(this).addClass("ApCloseBlock").addClass("Id-" + ResId).html(JsonTranslation[5].Value);
                                                                    ArrayRow[i] = 1;
                                                                }
                                                                else {
                                                                    $(this).addClass("Temporary").addClass("Id-" + ResId).html(JsonTranslation[4].Value);
                                                                    ArrayRow[i] = 1;
                                                                }
                                                            }
                                                            else {
                                                                if (Status == "A") {
                                                                    GetStream.addClass("BookedSlotes").html(JsonTranslation[0].Value).hide();
                                                                    if (ArrayRow[i] == undefined || ArrayRow[i] == 0) { ArrayRow[i] = 2 }
                                                                    else { ArrayRow[i] = ArrayRow[i] + 1; }
                                                                }
                                                                else if (Status == "C") {
                                                                    GetStream.addClass("ApCloseBlock").html(JsonTranslation[5].Value).hide();
                                                                    if (ArrayRow[i] == undefined || ArrayRow[i] == 0) { ArrayRow[i] = 2 }
                                                                    else { ArrayRow[i] = ArrayRow[i] + 1; }
                                                                }
                                                                else {
                                                                    GetStream.addClass("Temporary").html(JsonTranslation[4].Value).hide();
                                                                    if (ArrayRow[i] == undefined || ArrayRow[i] == 0) { ArrayRow[i] = 2 }
                                                                    else { ArrayRow[i] = ArrayRow[i] + 1; }
                                                                }

                                                            }

                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        if (!GetStream.hasClass("NotAvailable")) {
                                            for (i = 0; i <= QuickBlocking.length - 1; i++) {
                                                if (QuickBlocking[i] != "" && !GetStream.hasClass("NotAvailable")) {
                                                    var Consider = QuickBlocking[i].split(' ')[0];
                                                    if (parseInt(Consider) == parseInt(GetDates)) {
                                                        var BlockesTxt = QuickBlocking[i].replace('~', '').split(' ');
                                                        var Dtconsider = new Date(YearName, MonthName, BlockesTxt[0], BlockesTxt[1].split(':')[0], BlockesTxt[1].split(':')[1], 0, 0);
                                                        var DtAfter = new Date(YearName, MonthName, BlockesTxt[0], BlockesTxt[2].split(':')[0], BlockesTxt[2].split(':')[1], 0, 0);
                                                        var Status = QuickBlocking[i].split(" ")[3];
                                                        var UserId = $.trim(QuickBlocking[i].split(" ")[5]);
                                                        var ResId = QuickBlocking[i].split(" ")[4];
                                                        var BookingDate = QuickBlocking[i].split(" ")[6];
                                                        var Comment = QuickBlocking[i].split(" ")[7] != undefined && QuickBlocking[i].split(" ")[7] != "" ? (QuickBlocking[i].split(" ")[7]).toString().replace(/[*]/g, ' ') : JsonTranslation[7].Value;
                                                        var CurrentDatesTill = new Date(YearName, MonthName, SetDtMt(CurrentDates.getDate()), SetDtMt(CurrentDates.getHours()), SetDtMt(CurrentDates.getMinutes()) + SetDtMt(parseInt(Slot)), 0, 0);
                                                        var TempDate = "";
                                                        //if ((CurrentDates >= Dtconsider && CurrentDates < DtAfter) && UserId == $.trim($(this).prop("id").split('~')[1])) {
                                                        //    if (Status == "DA") {
                                                        //        var Html = "<div class='Quick_text'>" + JsonTranslation[7].Value + "</div> <div class='Quick_Edit'>" +
                                                        //                   "<i class='fa fa-pencil-square-o margin-right5' ResId=" + ResId + " UserId=" + UserId + " aria-hidden='true'></i><i class='fa fa-trash' ResId=" + ResId + " BookingDate=" + BookingDate + " aria-hidden='true'></i>" +
                                                        //                   "</div>";
                                                        //        $(this).addClass("Quick_Holiday").addClass("Id-" + ResId).html(Html);
                                                        //    }
                                                        //    //if (Status == "DA") {
                                                        //    //    $(this).addClass("holidayAdd").addClass("Id-" + ResId).html(JsonTranslation[3].Value);
                                                        //    //}
                                                        //}
                                                        if (((CurrentDates >= Dtconsider && CurrentDates < DtAfter) || TempDate == "Done") && !GetStream.hasClass("holidayAdd") && UserId == $.trim($(this).prop("id").split('~')[1])) {
                                                            var QuickHtml = "<div class='Quick_text'>" + Comment + "</div> <div class='Quick_Edit'>" +
                                                                         "<div><i class='fa fa-pencil-square-o margin-right5' ResId=" + ResId + " UserId=" + UserId + " aria-hidden='true'></i><i class='fa fa-trash' ResId=" + ResId + " BookingDate=" + BookingDate + " aria-hidden='true'></i></div>" +
                                                                         "</div>";
                                                            if (Dtconsider >= CurrentDates && Dtconsider < CurrentDatesTill) {
                                                                if (TempDate == "Done") {
                                                                    var Monthpls = parseInt(MonthName) + 1;
                                                                    QReplacingTxt[QReplacingCnt] = SetDtMt(CurrentDates.getDate()) + "-" + Monthpls + "-" + YearName + " " + SetDtMt(CurrentDates.getHours()) + ":" + SetDtMt(CurrentDates.getMinutes());
                                                                    QoldReplaceTxt[QReplacingCnt] = SetDtMt(Dtconsider.getDate()) + "-" + Monthpls + "-" + YearName + " " + SetDtMt(Dtconsider.getHours()) + ":" + SetDtMt(Dtconsider.getMinutes());
                                                                    QReplacingCnt = QReplacingCnt + 1;
                                                                    TempDate = "";
                                                                }
                                                                if (Status == "DA") {
                                                                    $(this).addClass("Quick_Holiday").addClass("Id-" + ResId).html(QuickHtml);
                                                                    QArrayRow[i] = 1;
                                                                }
                                                            }
                                                            else {
                                                                if (Status == "DA") {
                                                                    GetStream.addClass("Quick_Holiday").html(QuickHtml).hide();
                                                                    if (QArrayRow[i] == undefined || QArrayRow[i] == 0) { QArrayRow[i] = 2 }
                                                                    else { QArrayRow[i] = QArrayRow[i] + 1; }
                                                                }
                                                            }

                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        var DisplayMonth = parseInt(MonthName) + 1;
                                        //Identity
                                        GetStream.prop("id", GetDates + "-" + DisplayMonth + "-" + YearName + " " + Hour + ":" + Minutes + "~" + $.trim(GetStream.prop("id").split('~')[1]));
                                        if (!GetStream.hasClass("UnknownP") && !GetStream.hasClass("Quick_Holiday") && !GetStream.hasClass("BookedSlotes") && !GetStream.hasClass("NotAvailable") && !GetStream.hasClass("Temporary") && !GetStream.hasClass("ApCloseBlock") && !GetStream.hasClass("holidayAdd") && GetStream.prop("colspan") != 2) {
                                            GetStream.addClass("Available").html(JsonTranslation[1].Value);
                                        }
                                        for (i = 0; i < TimeSlotSet.length; i++) {
                                            if (TimeSlotSet[i].toString().indexOf(UId) != -1) {
                                                var SlotTemp = 0;
                                                if ($.inArray(UId, ArrayEmp) == -1) {
                                                    ArrayEmp[i] = UId;
                                                    ArrayEmpCount[i] = 0;
                                                    ArrayEmpDate[i] = new Date(YearName, MonthName, Day, Hour, Minutes, 0, 0);
                                                    SlotTemp = parseInt(TimeSlotSet[i].split(' ')[1]);
                                                    ArrayEmpDate[i].setMinutes(ArrayEmpDate[i].getMinutes() + SlotTemp);
                                                }
                                                else {
                                                    ArrayEmpCount[$.inArray(UId, ArrayEmp)] = ArrayEmpCount[$.inArray(UId, ArrayEmp)] + 1;
                                                    if (GetStream.hasClass("Available") && ArrayEmpDate[$.inArray(UId, ArrayEmp)] > CurrentDates) {
                                                        GetStream.removeClass("Available").addClass("NotAvailableStart").html(JsonTranslation[6].Value);
                                                    }
                                                    if (CurrentDates.getHours() == ArrayEmpDate[$.inArray(UId, ArrayEmp)].getHours() && CurrentDates.getMinutes() == ArrayEmpDate[$.inArray(UId, ArrayEmp)].getMinutes()) {
                                                        SlotTemp = parseInt(TimeSlotSet[i].split(' ')[1]);
                                                        ArrayEmpDate[$.inArray(UId, ArrayEmp)].setMinutes(ArrayEmpDate[$.inArray(UId, ArrayEmp)].getMinutes() + SlotTemp);
                                                    }
                                                }
                                                break;
                                            }
                                        }

                                        Tkscnt = Tkscnt + 1;
                                    }
                                });
                                //CountTr = CountTr + 1;
                            })
                            for (i = 0; i <= BlockingCheck.length - 1; i++) {
                                if (BlockingCheck[i] != "") {
                                    var DisplayMonth = parseInt(MonthName) + 1;
                                    var DatesFun = BlockingCheck[i].split(" ")[0];
                                    var HoursTemp = BlockingCheck[i].split(" ")[1].split(':')[0];
                                    var MinutesTemp = BlockingCheck[i].split(" ")[1].split(':')[1];
                                    var Name = (BlockingCheck[i].split(" ")[4]).toString().replace(/[*]/g, ' ');
                                    var UserId = BlockingCheck[i].split(" ")[6]
                                    var GoToElement = SetDtMt(DatesFun) + "-" + DisplayMonth + "-" + YearName + " " + HoursTemp + ":" + MinutesTemp;

                                    if ($.inArray(GoToElement, oldReplaceTxt) !== -1) {
                                        var PositionPlace = $.inArray(GoToElement, oldReplaceTxt);
                                        GoToElement = ReplacingTxt[PositionPlace];
                                    }
                                    var RowspanValue = "" + ArrayRow[i];
                                    if (!$("#DayCalBodyTag").find("[id='" + GoToElement + "~" + UserId + "']").hasClass("holidayAdd") && !$("#DayCalBodyTag").find("[id='" + GoToElement + "~" + UserId + "']").hasClass("NotAvailable")) {
                                        $("#DayCalBodyTag").find("[id='" + GoToElement + "~" + UserId + "']").prop("rowspan", RowspanValue).html(Name);
                                        //.addClass("Border-Book")
                                    }
                                }
                            }
                            for (i = 0; i <= QuickBlocking.length - 1; i++) {
                                if (QuickBlocking[i] != "") {
                                    var DisplayMonth = parseInt(MonthName) + 1;
                                    var DatesFun = QuickBlocking[i].split(" ")[0];
                                    var HoursTemp = QuickBlocking[i].split(" ")[1].split(':')[0];
                                    var MinutesTemp = QuickBlocking[i].split(" ")[1].split(':')[1];
                                    var UserId = $.trim(QuickBlocking[i].split(" ")[5]);
                                    var ResId = QuickBlocking[i].split(" ")[4];
                                    var BookingDate = QuickBlocking[i].split(" ")[6];
                                    var Comment = QuickBlocking[i].split(" ")[7] != undefined && QuickBlocking[i].split(" ")[7] != "" ? (QuickBlocking[i].split(" ")[7]).toString().replace(/[*]/g, ' ') : JsonTranslation[7].Value;
                                    var QuickHtml = "<div class='Quick_text'>" + Comment + "</div> <div class='Quick_Edit'>" +
                                                     "<div><i class='fa fa-pencil-square-o margin-right5' ResId=" + ResId + " UserId=" + UserId + " aria-hidden='true'></i><i class='fa fa-trash' ResId=" + ResId + " BookingDate=" + BookingDate + " aria-hidden='true'></i></div>" +
                                                     "</div>";
                                    var GoToElement = SetDtMt(DatesFun) + "-" + DisplayMonth + "-" + YearName + " " + HoursTemp + ":" + MinutesTemp;

                                    if ($.inArray(GoToElement, QoldReplaceTxt) !== -1) {
                                        var PositionPlace = $.inArray(GoToElement, QoldReplaceTxt);
                                        GoToElement = QReplacingTxt[PositionPlace];
                                    }
                                    var RowspanValue = "" + QArrayRow[i];
                                    if (!$("#DayCalBodyTag").find("[id='" + GoToElement + "~" + UserId + "']").hasClass("holidayAdd") && !$("#DayCalBodyTag").find("[id='" + GoToElement + "~" + UserId + "']").hasClass("NotAvailable")) {
                                        $("#DayCalBodyTag").find("[id='" + GoToElement + "~" + UserId + "']").prop("rowspan", RowspanValue).html(QuickHtml);
                                        //.addClass("Border-Book")
                                    }
                                }
                            }

                            var AvailCnt = parseInt($("#DayCalBodyTag").find(".Available").length) + parseInt($("#DayCalBodyTag").find(".NotAvailableStart").length);
                            var TotalCnt = $('#DayCalBodyTag tr td:not([colspan=2])').not("Quick_Holiday").not(".NotAvailable").not(".holidayAdd").not(".UnknownP").length;
                            var Unutilized = 0;
                            if (AvailCnt != 0 && TotalCnt != 0) {
                                Unutilized = parseInt((AvailCnt / TotalCnt) * 100);
                            }
                            $("#UnoccDay span").html(Unutilized + "%");
                            $(".loader").hide();
                        },
                        cache: false,
                    });
                    return false;
                })
            })
        },
        error: function (request, status, error) { }
    });
}
function GoDailyCalender() {
    $(".loader").show();
    var ArrayRow = new Array();
    var QArrayRow = new Array();
    var LangUsed = changeLanguage();
    var UserId = $("#EmployeesDaybtn .activebutton").prop("id");
    //LangChange
    moment.lang(LangUsed);
    var MonthName = parseInt(moment($.trim($(".Header-FullCalanderDaily").html().split('-')[0]), 'MMM').format('M')) - 1;
    var EngMonth = moment($.trim($(".Header-FullCalanderDaily").html().split('-')[0]), 'MMM', LangUsed).lang('en').format("MMMM");
    var YearName = $.trim($(".Header-FullCalanderDaily").html().split('-')[1]);
    var Day = ($.trim($(".DateLatestDaily").html())).split(' ')[1].split("-")[0];
    var statusDaily = "Daily";
    var FullDate = Day + "-" + (parseInt(MonthName) + 1) + "-" + YearName;
    if (UserId != "All") {
        $("#btnQuickBlocker").attr("BlockUserId", UserId);
        $("#btnQuickBlocker").show();
        $.ajax({
            url: "/Reservation/BlockDayDetail?UserId=" + UserId + "&GetDate=" + FullDate,
            type: 'POST',
            success: function (data) {
                $("#DayCalBodyTag").html(data);
                $.ajax({
                    url: "/Reservation/GetAllBookedData?UserId=" + UserId + "&Monthname=" + EngMonth + "&yearname=" + YearName + "&Day=" + ($.trim($(".DateLatestDaily").html())).split(' ')[1].split("-")[0] + "&Status=" + statusDaily,
                    type: 'POST',
                    dataType: "json",
                    async: true,
                    success: function (Blocking) {
                        $("#DailyRevenue span").html(Blocking.TotalRev.replace(/[*]/g, ' '));
                        var Lnth = $("#DayCalBodyTag").find('tr').length;
                        var BlockingCheck = Blocking.DataSlot;
                        var QuickBlocking = Blocking.QuickBlock;
                        var Daysga = Blocking.WeekdaysBlock;
                        var CountTr = 0;
                        var Slot = $("#DayCalBodyTag").find('tr').first().prop("class");
                        var ReplacingTxt = new Array();
                        var oldReplaceTxt = new Array();
                        var ReplacingCnt = 0;
                        var QReplacingTxt = new Array();
                        var QoldReplaceTxt = new Array();
                        var QReplacingCnt = 0;
                        $.ajax({
                            url: "/Reservation/getCalendarConverted",
                            cache: false,
                            dataType: "json",
                            success: function (jsonOutput) {
                                $.each($("#DayCalBodyTag tr"), function () {
                                    var count = 1;
                                    $.each($(this).children("td:not([colspan=2])"), function () {
                                        var GetStream = $(this);
                                        // var NewTag = $("#Weeks-Dates tr th:nth-child(" + count + ")");                
                                        var GetDates = ($.trim($(".DateLatestDaily").html())).split(' ')[1].split("-")[0];
                                        if (GetDates != "") {
                                            //Get the current Hour
                                            var Day = GetDates;
                                            var Hour = $(this).prop("id").split(':')[0];
                                            //Get the current Minute
                                            var Minutes = $(this).prop("id").split(':')[1];

                                            //Get the Date
                                            var CurrentDates = new Date(YearName, MonthName, Day, Hour, Minutes);
                                            //Not Available Days Block
                                            if (Daysga.indexOf(GetWeekNames(CurrentDates.getDay())) < 0) {
                                                $(this).addClass("NotAvailable").html(jsonOutput[2].Value);
                                            }
                                            //This hour of Day is not
                                            var AllDay = Blocking.NotAvailableDay;
                                            var AllTime = Blocking.NotAvailableTime;
                                            if ($.inArray(GetWeekNames(CurrentDates.getDay()), AllDay) !== -1) {
                                                var NotPos = $.inArray(GetWeekNames(CurrentDates.getDay()), AllDay);
                                                var Hourstart = AllTime[NotPos].split(' ')[0].split(':')[0];
                                                var Minutestart = AllTime[NotPos].split(' ')[0].split(':')[1];
                                                var HoursEnd = AllTime[NotPos].split(' ')[1].split(':')[0];
                                                var MinuteEnd = AllTime[NotPos].split(' ')[1].split(':')[1];
                                                var NoStart = new Date(YearName, MonthName, Day, Hourstart, Minutestart, 0, 0);
                                                var NoEnd = new Date(YearName, MonthName, Day, HoursEnd, MinuteEnd, 0, 0);
                                                if (CurrentDates >= NoStart && CurrentDates < NoEnd) { }
                                                else { $(this).addClass("NotAvailable").html(jsonOutput[2].Value); }
                                            }
                                            //HolidayBlock
                                            var Holiday = Blocking.employeelive;
                                            for (i = 0; i <= Holiday.length - 1; i++) {
                                                if (Holiday[i] != "") {
                                                    var ConsiderHoliday0 = parseInt(Holiday[i].split(' ')[0]);
                                                    var ConsiderHoliday1 = parseInt(Holiday[i].split(' ')[1]);
                                                    if (ConsiderHoliday0 >= parseInt(GetDates) || ConsiderHoliday0 <= parseInt(GetDates)) {
                                                        var BlockesTxt = Holiday[i].replace('~', '').split(' ');
                                                        var Dtconsider = new Date(YearName, MonthName, BlockesTxt[0], Hour, Minutes);
                                                        var DtAfter = new Date(YearName, MonthName, BlockesTxt[1], Hour, Minutes);
                                                        //&& !GetStream.hasClass("NotAvailable")
                                                        if (Dtconsider <= CurrentDates && DtAfter >= CurrentDates) {
                                                            GetStream.addClass("holidayAdd").html(jsonOutput[3].Value);
                                                        }
                                                    }
                                                }
                                            }
                                            //Block Bookings
                                            for (i = 0; i <= BlockingCheck.length - 1; i++) {
                                                if (BlockingCheck[i] != "") {
                                                    var Consider = BlockingCheck[i].split(' ')[0];
                                                    if (parseInt(Consider) == parseInt(GetDates)) {
                                                        var BlockesTxt = BlockingCheck[i].replace('~', '').split(' ');
                                                        var Dtconsider = new Date(YearName, MonthName, BlockesTxt[0], BlockesTxt[1].split(':')[0], BlockesTxt[1].split(':')[1], 0, 0);
                                                        var DtAfter = new Date(YearName, MonthName, BlockesTxt[0], BlockesTxt[2].split(':')[0], BlockesTxt[2].split(':')[1], 0, 0);
                                                        var Status = BlockingCheck[i].split(" ")[3];
                                                        var ResId = BlockingCheck[i].split(" ")[5];
                                                        var CurrentDatesTill = new Date(YearName, MonthName, SetDtMt(CurrentDates.getDate()), SetDtMt(CurrentDates.getHours()), SetDtMt(CurrentDates.getMinutes()) + SetDtMt(parseInt(Slot)), 0, 0);
                                                        var TempDate = "";
                                                        //if (CurrentDates < Dtconsider) {
                                                        //    if (CurrentDatesTill > Dtconsider) {
                                                        //        TempDate = "Done";
                                                        //    }
                                                        //}
                                                        if (((CurrentDates >= Dtconsider && CurrentDates < DtAfter) || TempDate == "Done") && !GetStream.hasClass("holidayAdd") && !GetStream.hasClass("NotAvailable")) {
                                                            //if (CurrentDates.getDate() == Dtconsider.getDate() && CurrentDates.getMonth() == Dtconsider.getMonth() && CurrentDates.getFullYear() == Dtconsider.getFullYear() && (Dtconsider.getHours() >= CurrentDates.getHours() && Dtconsider.getHours() < CurrentDatesTill.getHours()) && (Dtconsider.getMinutes() >= CurrentDates.getMinutes() && Dtconsider.getMinutes() < CurrentDatesTill.getMinutes())) {
                                                            if (Dtconsider >= CurrentDates && Dtconsider < CurrentDatesTill) {

                                                                if (TempDate == "Done") {
                                                                    var Monthpls = parseInt(MonthName) + 1;
                                                                    var MinutesTemp = SetDtMt(CurrentDates.getMinutes); var MinuteTemp = SetDtMt(Dtconsider.getMinutes); var HoursTemp = SetDtMt(CurrentDates.getHours); var HourTemp = SetDtMt(Dtconsider.getHours); var DatesTemp = SetDtMt(CurrentDates.getDate()); var DateTemp = SetDtMt(Dtconsider.getDate());
                                                                    //if (CurrentDates.getMinutes() <= 9) { MinutesTemp = "0" + CurrentDates.getMinutes(); }
                                                                    //else { MinutesTemp = CurrentDates.getMinutes() + ""; }
                                                                    //if (Dtconsider.getMinutes() <= 9) { MinuteTemp = "0" + Dtconsider.getMinutes(); }
                                                                    //else { MinuteTemp = Dtconsider.getMinutes() + ""; }
                                                                    //if (CurrentDates.getHours() <= 9) { HoursTemp = "0" + CurrentDates.getHours(); }
                                                                    //else { HoursTemp = CurrentDates.getHours() + ""; }
                                                                    //if (Dtconsider.getHours() <= 9) { HourTemp = "0" + Dtconsider.getHours(); }
                                                                    //else { HourTemp = Dtconsider.getHours() + ""; }
                                                                    //if (CurrentDates.getDate() <= 9) { DatesTemp = "0" + CurrentDates.getDate(); }
                                                                    //else { DatesTemp = CurrentDates.getDate() + ""; }
                                                                    //if (Dtconsider.getDate() <= 9) { DateTemp = "0" + Dtconsider.getDate(); }
                                                                    //else { DateTemp = Dtconsider.getDate() + ""; }
                                                                    ReplacingTxt[ReplacingCnt] = DatesTemp + "-" + Monthpls + "-" + YearName + " " + HoursTemp + ":" + MinutesTemp;
                                                                    oldReplaceTxt[ReplacingCnt] = DateTemp + "-" + Monthpls + "-" + YearName + " " + HourTemp + ":" + MinuteTemp;
                                                                    ReplacingCnt = ReplacingCnt + 1;
                                                                    TempDate = "";
                                                                }
                                                                if (Status == "A") {
                                                                    $(this).addClass("BookedSlotes").addClass("Id-" + ResId).html(jsonOutput[0].Value);
                                                                    ArrayRow[i] = 1;
                                                                }
                                                                else if (Status == "C") {
                                                                    $(this).addClass("ApCloseBlock").addClass("Id-" + ResId).html(jsonOutput[5].Value);
                                                                    ArrayRow[i] = 1;
                                                                }
                                                                else {
                                                                    $(this).addClass("Temporary").addClass("Id-" + ResId).html(jsonOutput[4].Value);
                                                                    ArrayRow[i] = 1;
                                                                }

                                                            }
                                                            else {
                                                                if (Status == "A") {
                                                                    GetStream.addClass("BookedSlotes").html(jsonOutput[0].Value).hide();
                                                                    if (ArrayRow[i] == undefined || ArrayRow[i] == 0) { ArrayRow[i] = 2 }
                                                                    else { ArrayRow[i] = ArrayRow[i] + 1; }
                                                                }
                                                                else if (Status == "C") {
                                                                    GetStream.addClass("ApCloseBlock").html(jsonOutput[5].Value).hide();
                                                                    if (ArrayRow[i] == undefined || ArrayRow[i] == 0) { ArrayRow[i] = 2 }
                                                                    else { ArrayRow[i] = ArrayRow[i] + 1; }
                                                                }
                                                                else {
                                                                    GetStream.addClass("Temporary").html(jsonOutput[4].Value).hide();
                                                                    if (ArrayRow[i] == undefined || ArrayRow[i] == 0) { ArrayRow[i] = 2 }
                                                                    else { ArrayRow[i] = ArrayRow[i] + 1; }
                                                                }

                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            if (!GetStream.hasClass("NotAvailable")) {
                                                for (i = 0; i <= QuickBlocking.length - 1; i++) {
                                                    if (QuickBlocking[i] != "" && !GetStream.hasClass("NotAvailable")) {
                                                        var Consider = QuickBlocking[i].split(' ')[0];
                                                        if (parseInt(Consider) == parseInt(GetDates)) {
                                                            var BlockesTxt = QuickBlocking[i].replace('~', '').split(' ');
                                                            var Dtconsider = new Date(YearName, MonthName, BlockesTxt[0], BlockesTxt[1].split(':')[0], BlockesTxt[1].split(':')[1], 0, 0);
                                                            var DtAfter = new Date(YearName, MonthName, BlockesTxt[0], BlockesTxt[2].split(':')[0], BlockesTxt[2].split(':')[1], 0, 0);
                                                            var Status = QuickBlocking[i].split(" ")[3];
                                                            var UserId = $.trim(QuickBlocking[i].split(" ")[5]);
                                                            var ResId = QuickBlocking[i].split(" ")[4];
                                                            var BookingDate = QuickBlocking[i].split(" ")[6];
                                                            var Comment = QuickBlocking[i].split(" ")[7] != undefined && QuickBlocking[i].split(" ")[7] != "" ? (QuickBlocking[i].split(" ")[7]).toString().replace(/[*]/g, ' ') : jsonOutput[7].Value;
                                                            var CurrentDatesTill = new Date(YearName, MonthName, SetDtMt(CurrentDates.getDate()), SetDtMt(CurrentDates.getHours()), SetDtMt(CurrentDates.getMinutes()) + SetDtMt(parseInt(Slot)), 0, 0);
                                                            var TempDate = "";
                                                            //if (CurrentDates >= Dtconsider && CurrentDates < DtAfter) {
                                                            //    if (Status == "DA") {
                                                            //        var Html = "<div class='Quick_text'>" + jsonOutput[7].Value + "</div> <div class='Quick_Edit'>" +
                                                            //                   "<i class='fa fa-pencil-square-o margin-right5' ResId=" + ResId + " UserId=" + UserId + " aria-hidden='true'></i><i class='fa fa-trash' ResId=" + ResId + " BookingDate=" + BookingDate + " aria-hidden='true'></i>" +
                                                            //                   "</div>";
                                                            //        $(this).addClass("Quick_Holiday").addClass("Id-" + ResId).html(Html);
                                                            //    }
                                                            //}
                                                            if (((CurrentDates >= Dtconsider && CurrentDates < DtAfter) || TempDate == "Done") && !GetStream.hasClass("holidayAdd") && !GetStream.hasClass("NotAvailable")) {
                                                                //if (CurrentDates.getDate() == Dtconsider.getDate() && CurrentDates.getMonth() == Dtconsider.getMonth() && CurrentDates.getFullYear() == Dtconsider.getFullYear() && (Dtconsider.getHours() >= CurrentDates.getHours() && Dtconsider.getHours() < CurrentDatesTill.getHours()) && (Dtconsider.getMinutes() >= CurrentDates.getMinutes() && Dtconsider.getMinutes() < CurrentDatesTill.getMinutes())) {
                                                                var QuickHtml = "<div class='Quick_text'>" + Comment + "</div> <div class='Quick_Edit'>" +
                                                                              "<div><i class='fa fa-pencil-square-o margin-right5' ResId=" + ResId + " UserId=" + UserId + " aria-hidden='true'></i><i class='fa fa-trash' ResId=" + ResId + " BookingDate=" + BookingDate + " aria-hidden='true'></i></div>" +
                                                                              "</div>";
                                                                if (Dtconsider >= CurrentDates && Dtconsider < CurrentDatesTill) {

                                                                    if (TempDate == "Done") {
                                                                        var Monthpls = parseInt(MonthName) + 1;
                                                                        var MinutesTemp = SetDtMt(CurrentDates.getMinutes); var MinuteTemp = SetDtMt(Dtconsider.getMinutes); var HoursTemp = SetDtMt(CurrentDates.getHours); var HourTemp = SetDtMt(Dtconsider.getHours); var DatesTemp = SetDtMt(CurrentDates.getDate()); var DateTemp = SetDtMt(Dtconsider.getDate());
                                                                        //if (CurrentDates.getMinutes() <= 9) { MinutesTemp = "0" + CurrentDates.getMinutes(); }
                                                                        //else { MinutesTemp = CurrentDates.getMinutes() + ""; }
                                                                        //if (Dtconsider.getMinutes() <= 9) { MinuteTemp = "0" + Dtconsider.getMinutes(); }
                                                                        //else { MinuteTemp = Dtconsider.getMinutes() + ""; }
                                                                        //if (CurrentDates.getHours() <= 9) { HoursTemp = "0" + CurrentDates.getHours(); }
                                                                        //else { HoursTemp = CurrentDates.getHours() + ""; }
                                                                        //if (Dtconsider.getHours() <= 9) { HourTemp = "0" + Dtconsider.getHours(); }
                                                                        //else { HourTemp = Dtconsider.getHours() + ""; }
                                                                        //if (CurrentDates.getDate() <= 9) { DatesTemp = "0" + CurrentDates.getDate(); }
                                                                        //else { DatesTemp = CurrentDates.getDate() + ""; }
                                                                        //if (Dtconsider.getDate() <= 9) { DateTemp = "0" + Dtconsider.getDate(); }
                                                                        //else { DateTemp = Dtconsider.getDate() + ""; }
                                                                        QReplacingTxt[QReplacingCnt] = DatesTemp + "-" + Monthpls + "-" + YearName + " " + HoursTemp + ":" + MinutesTemp;
                                                                        QoldReplaceTxt[QReplacingCnt] = DateTemp + "-" + Monthpls + "-" + YearName + " " + HourTemp + ":" + MinuteTemp;
                                                                        QReplacingCnt = QReplacingCnt + 1;
                                                                        TempDate = "";
                                                                    }
                                                                    if (Status == "DA") {
                                                                        $(this).addClass("Quick_Holiday").addClass("Id-" + ResId).html(QuickHtml);
                                                                        QArrayRow[i] = 1;
                                                                    }
                                                                }
                                                                else {
                                                                    if (Status == "DA") {
                                                                        GetStream.addClass("Quick_Holiday").html(QuickHtml).hide();
                                                                        if (QArrayRow[i] == undefined || QArrayRow[i] == 0) { QArrayRow[i] = 2 }
                                                                        else { QArrayRow[i] = QArrayRow[i] + 1; }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            var DisplayMonth = parseInt(MonthName) + 1;
                                            $(this).prop("id", GetDates + "-" + DisplayMonth + "-" + YearName + " " + Hour + ":" + Minutes);
                                            if (!$(this).hasClass("UnknownP") && !$(this).hasClass("Quick_Holiday") && !$(this).hasClass("BookedSlotes") && !$(this).hasClass("Temporary") && !$(this).hasClass("ApCloseBlock") && !$(this).hasClass("NotAvailable") && !$(this).hasClass("holidayAdd") && $(this).prop("colspan") != 2) {
                                                $(this).addClass("Available").html(jsonOutput[1].Value);
                                            }
                                        }
                                    });
                                    //CountTr = CountTr + 1;
                                })
                                for (i = 0; i <= BlockingCheck.length - 1; i++) {
                                    if (BlockingCheck[i] != "") {
                                        var DisplayMonth = parseInt(MonthName) + 1;
                                        var DatesFun = BlockingCheck[i].split(" ")[0];
                                        var HoursTemp = BlockingCheck[i].split(" ")[1].split(':')[0];
                                        var MinutesTemp = BlockingCheck[i].split(" ")[1].split(':')[1];
                                        var Name = (BlockingCheck[i].split(" ")[4]).toString().replace(/[*]/g, ' ');
                                        var GoToElement = SetDtMt(DatesFun) + "-" + DisplayMonth + "-" + YearName + " " + HoursTemp + ":" + MinutesTemp;
                                        if ($.inArray(GoToElement, oldReplaceTxt) !== -1) {
                                            var PositionPlace = $.inArray(GoToElement, oldReplaceTxt);
                                            GoToElement = ReplacingTxt[PositionPlace];
                                        }
                                        var RowspanValue = "" + ArrayRow[i];
                                        if (!$("#DayCalBodyTag").find("[id='" + GoToElement + "']").hasClass("holidayAdd")) {
                                            $("#DayCalBodyTag").find("[id='" + GoToElement + "']").prop("rowspan", RowspanValue).html(Name);
                                            //.addClass("Border-Book")
                                        }
                                    }
                                }
                                for (i = 0; i <= QuickBlocking.length - 1; i++) {
                                    if (QuickBlocking[i] != "") {
                                        var DisplayMonth = parseInt(MonthName) + 1;
                                        var DatesFun = QuickBlocking[i].split(" ")[0];
                                        var HoursTemp = QuickBlocking[i].split(" ")[1].split(':')[0];
                                        var MinutesTemp = QuickBlocking[i].split(" ")[1].split(':')[1];
                                        var UserId = $.trim(QuickBlocking[i].split(" ")[5]);
                                        var ResId = QuickBlocking[i].split(" ")[4];
                                        var BookingDate = QuickBlocking[i].split(" ")[6];
                                        var Comment = QuickBlocking[i].split(" ")[7] != undefined && QuickBlocking[i].split(" ")[7] != "" ? (QuickBlocking[i].split(" ")[7]).toString().replace(/[*]/g, ' ') : jsonOutput[7].Value;
                                        var QuickHtml = "<div class='Quick_text'>" + Comment + "</div> <div class='Quick_Edit'>" +
                                                        "<div><i class='fa fa-pencil-square-o margin-right5' ResId=" + ResId + " UserId=" + UserId + " aria-hidden='true'></i><i class='fa fa-trash' ResId=" + ResId + " BookingDate=" + BookingDate + " aria-hidden='true'></i></div>" +
                                                        "</div>";
                                        var GoToElement = SetDtMt(DatesFun) + "-" + DisplayMonth + "-" + YearName + " " + HoursTemp + ":" + MinutesTemp;
                                        if ($.inArray(GoToElement, QoldReplaceTxt) !== -1) {
                                            var PositionPlace = $.inArray(GoToElement, QoldReplaceTxt);
                                            GoToElement = QReplacingTxt[PositionPlace];
                                        }
                                        var RowspanValue = "" + QArrayRow[i];
                                        if (!$("#DayCalBodyTag").find("[id='" + GoToElement + "']").hasClass("holidayAdd")) {
                                            $("#DayCalBodyTag").find("[id='" + GoToElement + "']").prop("rowspan", RowspanValue).html(QuickHtml);
                                            //.addClass("Border-Book")
                                        }
                                    }
                                }
                                var AvailCnt = $("#DayCalBodyTag").find(".Available").length;
                                var TotalCnt = $('#DayCalBodyTag tr td:not([colspan=2])').not(".Quick_Holiday").not(".NotAvailable").not(".holidayAdd").not(".UnknownP").length;
                                var Unutilized = 0;
                                if (AvailCnt != 0 && TotalCnt != 0) {
                                    Unutilized = parseInt((AvailCnt / TotalCnt) * 100);
                                }
                                $("#UnoccDay span").html(Unutilized + "%");
                                $(".loader").hide();
                            },
                            error: function (request, status, error) { }
                        });
                    },
                    cache: false,
                    contentType: false,
                    processData: false
                });
            },
            cache: false,
        });
        return false;
    }
    else {
        $("#btnQuickBlocker").hide();
        GoAllemployeeDetails();
    }
}
function GoAdd() {
    $(".loader").show();
    //var ArrayRow = new Array();   
    var UserId = $("#Employeesbtn .activebutton").prop("id");
    $("#btnMonthQuickBlocker").attr("BlockUserId", UserId);
    $("#btnMonthQuickBlocker").show();
    //LangChange
    var LangUsed = changeLanguage();
    moment.lang(LangUsed);
    var MonthName = parseInt(moment($.trim($(".Header-FullCalander").html().split('-')[0]), 'MMM').format('M')) - 1;
    var EngMonth = moment($.trim($(".Header-FullCalander").html().split('-')[0]), 'MMM', LangUsed).lang('en').format("MMMM");
    var YearName = $.trim($(".Header-FullCalander").html().split('-')[1]);
    var status = "Month";
    $.post("/Reservation/BlockNewCalenderMonthDetail?UserId=" + UserId + "&Monthname=" + EngMonth + "&yearname=" + YearName, function (data) {
        $("#BodyTag").html(data);

        //$.post("/Reservation/GetAllBookedData?UserId=" + UserId + "&Monthname=" + EngMonth + "&yearname=" + YearName + "&Status=" + status, function (Blocking) {        
        //    $("#MonthlyRevenue span").html(Blocking.split('/')[3].toString().replace(/[*]/g, ' '));
        //    BlockBookingCalander(MonthName, YearName, Blocking, "Monthly");
        //});
        $.ajax({
            url: "/Reservation/GetAllBookedData?UserId=" + UserId + "&Monthname=" + EngMonth + "&yearname=" + YearName + "&Status=" + status,
            type: 'POST',
            dataType: "json",
            async: true,
            success: function (Blocking) {
                $("#MonthlyRevenue span").html(Blocking.TotalRev.replace(/[*]/g, ' '));
                BlockBookingCalander(MonthName, YearName, Blocking, "Monthly");
            },
            cache: false,
        });
        return false;
    })
}
function GoWeekCalender() {
    $(".loader").show();
    var UserId = $("#EmployeesWeekbtn .activebutton").prop("id");
    $("#btnWeekQuickBlocker").attr("BlockUserId", UserId);
    $("#btnWeekQuickBlocker").show();
    //LanguageChange
    var LangUsed = changeLanguage();
    moment.lang(LangUsed);
    var MonthName = parseInt(moment($.trim($(".WeekDatesDisp").html().split('-')[0]), 'MMM').format('M')) - 1;
    var EngMonth = moment($.trim($(".WeekDatesDisp").html().split('-')[0]), 'MMM', LangUsed).lang('en').format("MMMM");
    var YearName = $.trim($(".WeekDatesDisp").html().split('-')[1]);
    var FirstDate = $("#Weeks-Dates tr th").not("[colspan='2']").not(":empty").first().html();
    var FinalDate = $("#Weeks-Dates tr th.Final-Date").html();
    var statusWeek = "Week";
    $.post("/Reservation/BlockWeekDetails?UserId=" + UserId, function (data) {
        $("#WeekCalBodyTag").html(data);
        //$.post("/Reservation/GetAllBookedData?UserId=" + UserId + "&Monthname=" + EngMonth + "&yearname=" + YearName + "&FirstDate=" + FirstDate + "&Finaldate=" + FinalDate + "&Status=" + statusWeek, function (Blocking) {
        //    var Weeklyrevenue = Blocking.split('/')[3].toString().replace(/[*]/g, ' ');
        //    $("#WeeklyRevenue span").html(Weeklyrevenue);
        //    BlockBookingCalander(MonthName, YearName, Blocking, "Weekly");
        //});
        $.ajax({
            url: "/Reservation/GetAllBookedData?UserId=" + UserId + "&Monthname=" + EngMonth + "&yearname=" + YearName + "&FirstDate=" + FirstDate + "&Finaldate=" + FinalDate + "&Status=" + statusWeek,
            type: 'POST',
            dataType: "json",
            async: true,
            success: function (Blocking) {
                var Weeklyrevenue = Blocking.TotalRev.replace(/[*]/g, ' ');
                $("#WeeklyRevenue span").html(Weeklyrevenue);
                BlockBookingCalander(MonthName, YearName, Blocking, "Weekly");
            },
            cache: false,
        });
        return false;
    })
}
function ShowDataWeeklyCalender(UserId) {
    var MonthName = parseInt(moment($.trim($(".WeekDatesDisp").html().split('-')[0]), 'MMM').format('M')) - 1;
    var YearName = $.trim($(".WeekDatesDisp").html().split('-')[1]);
    var Day = $("#Weeks-Dates tr th.Final-Date").html().split('-')[0];
    var AllEmployee_Date = Day + "-" + MonthName + "-" + YearName;
    if ($("#EmployeesWeekbtn .activebutton").prop("id") != UserId) {
        $("#EmplyeeDetails").html("");
        $("#EmployeesWeekbtn a").removeClass("activebutton");
        $("#EmployeesDaybtn a").removeClass("activebutton");
        $("#Employeesbtn a").removeClass("activebutton");
        $("#EmployeesWeekbtn #" + UserId).addClass("activebutton");
        $("#EmployeesDaybtn #" + UserId).addClass("activebutton");
        $("#Employeesbtn #" + UserId).addClass("activebutton");
        $("#UserNamesForWeek").html($("#EmployeesWeekbtn .activebutton span").html());
        $("#UserNames").html($("#EmployeesDaybtn .activebutton span").html());
        $("#UserNamesForMonth").html($("#Employeesbtn .activebutton span").html());
        GetAllDefaultsForWeek(AllEmployee_Date);
        GoWeekCalender();
        GoAdd();
        GoDailyCalender();
    }
}
function DailyDefault(AllEmployee_Date) {
    var LangUsed = changeLanguage();
    //GetCurrent Date
    var CurDateDaily = new Date();
    if (AllEmployee_Date != null && AllEmployee_Date != "" && AllEmployee_Date != undefined) {
        CurDateDaily.setDate(parseInt(AllEmployee_Date.split('-')[0]));
        CurDateDaily.setMonth(parseInt(AllEmployee_Date.split('-')[1]));
        CurDateDaily.setFullYear(parseInt(AllEmployee_Date.split('-')[2]));
    }
    var LangaugeMonth = moment(CurDateDaily.getFullYear() + "-" + SetDtMt((CurDateDaily.getMonth() + 1)) + "-" + SetDtMt(CurDateDaily.getDate())).lang(LangUsed).format('MMMM');
    var LangaugeWeek = moment(CurDateDaily.getFullYear() + "-" + SetDtMt((CurDateDaily.getMonth() + 1)) + "-" + SetDtMt(CurDateDaily.getDate())).lang(LangUsed).format('dddd');
    //Main Header For Month and Year
    $(".Header-FullCalanderDaily").html(LangaugeMonth + " - " + CurDateDaily.getFullYear());
    //Sub Title for Full Current Date
    $(".DateLatestDaily").html(LangaugeWeek + " " + SetDtMt(CurDateDaily.getDate()) + "-" + LangaugeMonth + "-" + CurDateDaily.getFullYear());
    GoDailyCalender();
}
function ShowDataDailyCalender(UserId) {
    var MonthName = parseInt(moment($.trim($(".Header-FullCalanderDaily").html().split('-')[0]), 'MMM').format('M')) - 1;
    var YearName = $.trim($(".Header-FullCalanderDaily").html().split('-')[1]);
    var Day = ($.trim($(".DateLatestDaily").html())).split(' ')[1].split("-")[0];
    var AllEmployee_Date = Day + "-" + MonthName + "-" + YearName;
    if (UserId != "All") {
        $("#EmplyeeDetails").html("");
        if ($("#EmployeesDaybtn .activebutton").prop("id") != UserId) {
            $("#EmployeesDaybtn a").removeClass("activebutton");
            $("#EmployeesWeekbtn a").removeClass("activebutton");
            $("#Employeesbtn a").removeClass("activebutton");
            $("#EmployeesDaybtn #" + UserId).addClass("activebutton");
            $("#EmployeesWeekbtn #" + UserId).addClass("activebutton");
            $("#Employeesbtn #" + UserId).addClass("activebutton");
            $("#UserNames").html($("#EmployeesDaybtn .activebutton span").html());
            $("#UserNamesForWeek").html($("#EmployeesWeekbtn .activebutton span").html());
            $("#UserNamesForMonth").html($("#Employeesbtn .activebutton span").html());
            DailyDefault(AllEmployee_Date);
            GoWeekCalender();
            GoAdd();
        }
    }
    else {
        $("#EmployeesDaybtn a").removeClass("activebutton");
        $("#EmployeesDaybtn #All").addClass("activebutton");
        $("#UserNames").html($("#EmployeesDaybtn .activebutton span").html());
        DailyDefault(AllEmployee_Date);
    }

}
function ShowAllButton(Userid) {
    $("#DayBTN").click();
    ShowDataDailyCalender(Userid);
}
function dataDisplayofTimeSlot(DateOnly) {
    $(".loader").show();
    var UserId = $("#UserIdOrder").val();
    var date = DateOnly;
    var OtherDate = moment($.trim($(".k-caption").html().split(',')[0]), 'MMM').lang('en').format("MMMM") + ", " + $.trim($(".k-caption").html().split(',')[1]);
    //var OtherDate = $(".k-caption").html();
    var FullDate = DateOnly;
    //var URLPATH = "/Home/GetUserTimeSlot?UserId=" + UserId + "&Day=" + FullDate;
    var URLPATH = "/Home/GetTimeForEmployee?startDateTime=" + FullDate + "&UserId=" + UserId + "&Durations=" + $("#DisplayList").val();
    $.ajax({
        url: URLPATH,
        beforeSend: function () {
            if (!$(".loader").is(":visible"))
                $(".loader").show();
        },
        type: 'POST',
        async: true,
        success: function (data) {
            //$("#TimeSlotEmployeeDisplay").show();
            //$("#TimeSlotDataForm").show();
            //$("#TimeSlotDataForm").html(data);
            //CheckTimeToBlock(UserId, DateOnly)
            $("#TimeSlotEmployeeDisplay").show();
            $("#TimeSlotDataForm").show();
            $("#TimeSlotDataForm").html(data);
        },
        complete: function () {
            $(".loader").hide();
        },
        cache: false,
        contentType: false,
        processData: false
    });
}
function DailyChangeCalanderMonth(Inc, year) {
    if ($(".loader").not(":visible"))
        $(".loader").show();
    //Get old selected Month
    var oldEngMonth = moment($.trim($(".Header-FullCalanderDaily").html().split('-')[0]), 'MMM').lang('en').format('MMMM');
    var MnthDaily = oldEngMonth;
    //Get Old selected Year
    var YearDaily = $.trim($(".Header-FullCalanderDaily").html().split('-')[1]);
    var GetReqMonth = (monthsList.indexOf(MnthDaily));
    var parsed_tomorrow = new Date();
    var LangUsed = changeLanguage();
    if (year == 2) {
        var DatePresent = parseInt($.trim($(".DateLatestDaily").html().split(' ')[1].split('-')[0]));
        parsed_tomorrow = new Date(YearDaily, GetReqMonth, DatePresent);
        parsed_tomorrow.setDate(SetDtMt(parsed_tomorrow.getDate() + Inc));
    }
    else {
        var Months = (GetReqMonth + 1) + Inc;
        var MaxPossible = daysInMonth(Months, YearDaily);
        parsed_tomorrow = new Date(YearDaily, Months - 1, 1);
    }
    var LangaugeMonth = moment(parsed_tomorrow.getFullYear() + "-" + SetDtMt((parsed_tomorrow.getMonth() + 1)) + "-" + SetDtMt(parsed_tomorrow.getDate())).lang(LangUsed).format('MMMM');
    var LangaugeWeek = moment(parsed_tomorrow.getFullYear() + "-" + SetDtMt((parsed_tomorrow.getMonth() + 1)) + "-" + SetDtMt(parsed_tomorrow.getDate())).lang(LangUsed).format('dddd');
    $(".Header-FullCalanderDaily").html(LangaugeMonth + " - " + parsed_tomorrow.getFullYear());
    $(".DateLatestDaily").html(LangaugeWeek + " " + SetDtMt(parsed_tomorrow.getDate()) + "-" + LangaugeMonth + "-" + parsed_tomorrow.getFullYear());
    GoDailyCalender();
}
function changedata(Inc, year) {
    var LangUsed = changeLanguage();
    var Monthsname = moment($.trim($(".Header-FullCalander").html().split('-')[0]), 'MMM', LangUsed).lang('en').format("MMMM");
    //var EngMonth = moment($.trim($(".Header-FullCalander").html().split('-')[0]), 'MMM', 'fr').lang('en').format("MMMM");
    var YearRateDesc = parseInt($.trim($(".Header-FullCalander").html().split('-')[1]));
    //It Can Sub or Add Month
    var GetReqMonth = (monthsList.indexOf(Monthsname));
    var MaxPossible = daysInMonth(GetReqMonth + 1, YearRateDesc);
    var parsed_tomorrow = new Date();
    if (Inc == 1) {
        parsed_tomorrow = new Date(YearRateDesc, GetReqMonth, MaxPossible);
    }
    else {
        parsed_tomorrow = new Date(YearRateDesc, GetReqMonth, 1);
    }
    if (year == 0) {
        parsed_tomorrow.setDate(SetDtMt(parsed_tomorrow.getDate() + Inc));

    }
    else {
        parsed_tomorrow.setFullYear(parsed_tomorrow.getFullYear() + Inc);
    }
    var LangaugeMonth = moment(parsed_tomorrow.getFullYear() + "-" + SetDtMt((parsed_tomorrow.getMonth() + 1)) + "-" + SetDtMt(parsed_tomorrow.getDate())).lang(LangUsed).format('MMMM');
    $(".Header-FullCalander").html(LangaugeMonth + " - " + parsed_tomorrow.getFullYear());
    var start_date = new Date(parsed_tomorrow.getFullYear(), parsed_tomorrow.getMonth(), 1);
    //Day of StartDate
    var start_withdate = start_date.getDay();
    //No. of possible dates in month
    var MaxDays = daysInMonth(start_date.getMonth() + 1, start_date.getFullYear());
    var Counting_Data = 0;
    var Count_Continue = 1;
    $("#dayCalc tr th:not(.DayTitle)").html("");
    //To fill Dates
    $.each($("#dayCalc tr th"), function () {
        if (!$(this).hasClass("DayTitle")) {
            if ($(this).prop("id") == "dates_" + start_withdate) {
                Counting_Data = 1;
            }
            if (Counting_Data == 1 && Count_Continue <= MaxDays) {
                $(this).html(Count_Continue);
                Count_Continue = Count_Continue + 1;
            }
            if (Counting_Data == 1 && Count_Continue > MaxDays) {
                return false;
            }
        }
    });
    GoAdd();
}
function GetWeekNumber(CurrentDates) {
    var InitialDate = new Date(CurrentDates.getFullYear(), CurrentDates.getMonth(), 1);
    var DatetobeTaken = InitialDate.getDay();
    if (DatetobeTaken == 0) { DatetobeTaken = 7; }
    var InitialRemaining = (7 - DatetobeTaken) + InitialDate.getDate();
    var FindWeek = Math.ceil((((CurrentDates.getDate() - InitialRemaining) / 7) + 1));
    return FindWeek;
}
function daysInMonth(month, year) {
    return new Date(year, month, 0).getDate();
}
function FindWeekSchedule(CurrentDates) {

    var WksDisp = "";
    //Get Week Number
    var CurrentDay = 0;
    var InitDate = new Date(CurrentDates.getFullYear(), SetDtMt(CurrentDates.getMonth()), 1);
    var CurrentDay = InitDate.getDay();
    if (CurrentDay == 0) { CurrentDay = 7; }
    var InitialRemaining = (7 - CurrentDay) + InitDate.getDate();
    var FindMaxMonth = daysInMonth(CurrentDates.getMonth() + 1, CurrentDates.getFullYear());
    var SundayArray = new Array(6);
    var counts = 0;
    for (i = InitialRemaining; i <= FindMaxMonth; i++) {
        SundayArray[counts] = i;
        i = i + 6;
        counts = counts + 1;
    }
    counts = 0;
    for (i = 0; i <= SundayArray.length; i++) {
        if (SundayArray[i] >= CurrentDates.getDate()) {
            var Currentstart = SundayArray[i] - 6;
            $("#Weeks-Dates tr .Dates").html("");
            $.each($(".Week-Dates tr th:not(.DateTitle)"), function () {
                var GetTitles = $(this);

                if (Currentstart > 0) {
                    var Months = CurrentDates.getMonth() + 1;
                    GetTitles.html(parseInt(Currentstart) + "-" + SetDtMt(parseInt(Months)) + "-" + CurrentDates.getFullYear());
                }
                Currentstart = Currentstart + 1;
                if (Currentstart > SundayArray[i]) {
                    $("#Weeks-Dates tr th").removeClass("Final-Date");
                    GetTitles.addClass("Final-Date");
                    return false;
                }

            });
            return false;
        }
        else if (CurrentDates.getDate() > SundayArray[i] && CurrentDates.getDate() <= FindMaxMonth && SundayArray[i] >= FindMaxMonth - 7) {
            var Currentstart = SundayArray[i] + 1;
            $("#Weeks-Dates tr .Dates").html("");
            $.each($(".Week-Dates tr th"), function () {
                var GetDetTitle = $(this);
                if (!$(this).hasClass("DateTitle")) {
                    if (Currentstart > 0) {
                        var Months = SetDtMt(CurrentDates.getMonth() + 1);
                        GetDetTitle.html(parseInt(Currentstart) + "-" + SetDtMt(parseInt(Months)) + "-" + CurrentDates.getFullYear());
                    }
                    Currentstart = Currentstart + 1;
                    if (Currentstart > FindMaxMonth) {
                        $("#Weeks-Dates tr th").removeClass("Final-Date");
                        GetDetTitle.addClass("Final-Date");
                        return false;
                    }
                }
            });
            return false;
        }
    }

}
function WeekChange(Position) {

    var LangUsed = changeLanguage();
    var Dates = parseInt($.trim(($("#Weeks-Dates .Final-Date").html()).split('-')[0]));
    var Month = parseInt($.trim(($("#Weeks-Dates .Final-Date").html()).split('-')[1])) - 1;
    var Year = parseInt($.trim(($("#Weeks-Dates .Final-Date").html()).split('-')[2]));
    var DatesSet = new Date(Year, SetDtMt(Month), SetDtMt(Dates));
    //DatesSet.setDate(Dates);
    //DatesSet.setMonth(Month);
    //DatesSet.setFullYear(Year);
    if (Position == -1) {
        Position = -7;
    }
    if (DatesSet.getDate() + Position < -1) {
        Position = 1 - DatesSet.getDate() - 1;
        DatesSet.setDate(DatesSet.getDate() + Position);
    }
    else {
        DatesSet.setDate(DatesSet.getDate() + Position);
    }
    FindWeekSchedule(DatesSet);
    var CurWeek = GetWeekNumber(DatesSet);
    var LangaugeMonth = moment(DatesSet.getFullYear() + "-" + SetDtMt((DatesSet.getMonth() + 1)) + "-" + SetDtMt(DatesSet.getDate())).lang(LangUsed).format('MMMM');
    $(".WeekNumbersDisplay span").html(" " + CurWeek);
    $(".WeekDatesDisp").html(LangaugeMonth + " - " + DatesSet.getFullYear());
    GoWeekCalender();
}
/*End Calander Script*/
function VariousTabColorChange(Colorid) {
    $(".loader").show();
    $.post("/Shop/BackgroundColorChange?ColorId=" + Colorid, function () {
        if (window.location.toString().indexOf('#') != -1) {
            var ab = window.location.hash.substr(1);
            if (ab == "Various") {
                location.reload();
            }
            else {
                window.location.hash = 'Various';
                window.location.reload();
            }
        }
        else {
            window.location.hash = '#Various';
            window.location.reload();
        }
    });

}
function GetAfterBlockEffect() {
    $(".loader").hide();
    var EmpUserId = $("#EmployeeChoosed").val();
    var Total = parseInt($("#TotalDuration").val());
    var Date = $(".k-days .k-selected").attr("data-date");
    var TempImplement = "";
    var Slot = 0;
    $.post("/Home/CheckDurationDifference?EmpUserId=" + EmpUserId + "&Date=" + Date, function (data) {
        Slot = parseInt(data);
        $.each($("#TimeSlotDataForm a"), function () {
            var b = Total;
            var abmain = $(this);
            var a = $(this).attr("name");
            var SlotOrigin = Slot;
            var Blocked = "";
            var i = 0;
            if (!abmain.hasClass("RemoveActivate")) {
                $.each($("#TimeSlotDataForm a"), function () {
                    var Last = $("#TimeSlotDataForm a").last().prop("name");
                    if (b > 9000 && b <= 0) { return false; }
                    // var MainAllEmp = $(this);
                    var GetDataEmp = $(this).attr("name");
                    if (GetDataEmp == a) { i = 1; }
                    if (Last == GetDataEmp) { SlotOrigin = 0; }
                    if (i == 1) {
                        if (b > 0) {
                            if (!$(this).hasClass("RemoveActivate")) {
                                b = b - SlotOrigin;
                            }
                            else {
                                b = 100000;
                            }
                        }
                    }
                });
                if (b > 0) {
                    abmain.addClass("PointerReplaceTemp").addClass("RemoveActivateTemp").unbind().unbind("mouseenter mouseleave mouseover mouseout").hide();
                }
            }
        });
        $(".loader").hide();
    });
}
function dataDisplayofTimeSlotUpdate(DateOnly, ReservationId) {
    $(".loader").show();
    var UserId = $("#UserIdOrder").val();
    if (UserId == "") {
    }
    else {
    }
    var OtherDate = moment($.trim($(".k-caption").html().split(',')[0]), 'MMM').lang('en').format("MMMM") + ", " + $.trim($(".k-caption").html().split(',')[1]);
    var FullDate = DateOnly;
    var URLPATH = "/Home/GetTimeForEmployee?startDateTime=" + FullDate + "&UserId=" + UserId + "&Durations=" + $("#TotalDuration").val() + "&ReservationId=" + ReservationId;
    $.ajax({
        url: URLPATH,
        beforeSend: function () {
            if (!$(".loader").is(":visible"))
                $(".loader").show();
        },
        type: 'POST',
        async: true,
        success: function (data) {
            //$("#TimeSlotEmployeeDisplay").show();
            //$("#TimeSlotDataForm").show();
            //$("#TimeSlotDataForm").html(data);
            //CheckTimeToBlock(UserId, DateOnly)
            $("#TimeSlotEmployeeDisplay").show();
            $("#TimeSlotDataForm").show();
            $("#TimeSlotDataForm").html(data);
        },
        complete: function () {
            $(".loader").hide();
        },
        cache: false,
        contentType: false,
        processData: false
    });
}
function GoUpOrder(index, Id, otherId, Groupstatus) {
    $(".loader").show();
    $.post("/Product/GoUp?index=" + index + "&Id=" + Id + "&otherId=" + otherId + "&Status=" + Groupstatus, function () {
        ShowProductDetail();
    });
}
function GoDownOrder(index, Id, otherId, Status) {
    $(".loader").show();
    $.post("/Product/GoDown?index=" + index + "&Id=" + Id + "&otherId=" + otherId + "&Status=" + Status, function () {
        ShowProductDetail();
    });
}
function CancelCalBookedDetails(Reservationid, Status, bookingdate) {
    $(".loader").show();
    var LangText = "Do You want to Cancel Booking?";
    var LangId = 5;
    if (Status == "QuickBlocker") {
        LangText = "Do You want to Cancel Leave?";
        LangId = 75;
    }
    $.post("/Product/GetDataOfLanguage?Text='" + LangText + "'&id=" + LangId, function (data) {
        $(".loader").hide();
        var RemoveCalBookedDetails = confirm(data);
        if (RemoveCalBookedDetails) {
            $(".loader").show();
            var Calview = $("#calendarmonth .view-cal .active").prop("id");
            var view = "";
            if (Calview == "DayBTN") {
                view = "Day";
            }
            if (Calview == "WeekBTN") {
                view = "Week";
            }
            if (Calview == "MONTHBTN") {
                view = "Month";
            }
            var AllView = $("#EmployeesDaybtn .activebutton").prop("id");
            var AllviewStatus = "";
            if (AllView == "All" && view == "Day") {
                AllviewStatus = "All";
            }
            $.post("/Reservation/CancelCalBookedDetail?status=" + Status + "&ReservationId=" + Reservationid + "&Viewstatus=" + view + "&bookingdate=" + bookingdate + "&Allview=" + AllviewStatus, function () {
                if (window.location.toString().indexOf('#') != -1) {
                    var ab = window.location.hash.substr(1);
                    if (ab == "calendarmonth") {
                        $('#welcome').modal("hide");
                        location.reload();
                    }
                    else {
                        $('#welcome').modal("hide");
                        window.location.hash = 'calendarmonth';
                        window.location.reload();
                    }
                }
                else {
                    $('#welcome').modal("hide");
                    window.location.hash = '#calendarmonth';
                    window.location.reload();
                }

            });
        }
        else {
            window.location.href = "/Reservation/Reservation#calendarmonth";
        }
    });
}
function CheckTimeToBlock(UserId, DateOnly) {
    //var getdate = DateOnly + " " + $(".k-caption").prop("id")
    var getdate = DateOnly;
    $.ajax({
        url: "/Home/GetTimeBlocking?UserId=" + UserId + "&Date=" + getdate,
        cache: false,
        dataType: "json",
        success: function (data) {
            var dataArray1 = data.BlockData;
            $(dataArray1).each(function (index, value) {
                $.each($("#TimeSlotEmployeeDisplay div a"), function () {
                    if ($(this).html() == value) {
                        if (!$(this).hasClass("RemoveActivate")) {
                            $(this).addClass("RemoveActivate").unbind().unbind("mouseenter mouseleave mouseover mouseout").addClass("PointerReplace").hide();
                        }
                    }
                });
            });
            GetAfterBlockEffect();
            //$(".loader").hide();

        },
        error: function (request, status, error) { }
    });
}
function CheckTimeToBlockUpdate(UserId, DateOnly, ReservationId) {
    //var getdate = DateOnly + " " + $(".k-caption").prop("id")
    //var getdate = DateOnly + " " + moment($.trim($(".k-caption").html().split(',')[0]), 'MMM').lang('en').format("MMMM") + ", " + $.trim($(".k-caption").html().split(',')[1]);
    var getdate = DateOnly;
    $.ajax({
        url: "/Home/GetTimeBlockingUpdate?UserId=" + UserId + "&Date=" + getdate + "&ReservationId=" + ReservationId,
        cache: false,
        dataType: "json",
        success: function (data) {
            var dataArray1 = data.BlockData;
            $(dataArray1).each(function (index, value) {
                $.each($("#TimeSlotEmployeeDisplay div a"), function () {
                    if ($(this).html() == value) {
                        if (!$(this).hasClass("RemoveActivate")) {
                            $(this).addClass("RemoveActivate").unbind().unbind("mouseenter mouseleave mouseover mouseout").addClass("PointerReplace").hide();
                        }
                    }
                });
            });
            GetAfterBlockEffect();
            //$(".loader").hide();
        },
        error: function (request, status, error) { }
    });



}
function GetAnyDefaultuser() {
    $("#AllImage div").removeClass("active");
    $("#DefaultUser").addClass("active");
    $("#TimeSlotDataForm").hide();
    $("#DateTimeDisplayEmployee").show();
}

function DeleteEmployeeProduct(solutionId) {
    $(".loader").show();
    $.post("/Product/GetDataOfLanguage?Text='Do You Want To Delete Employee Product?'&id=1", function (data) {
        $(".loader").hide();
        var ConfirmEmpProduct = confirm(data);
        if (ConfirmEmpProduct) {
            var UserId = $("#ProductUserId").val();
            if (UserId == "") {
                UserId = $("#UserId").val();
            }
            $(".loader").show();
            $.post("/Employee/DeleteEmployeeProduct?solutionId=" + solutionId, function () {
                EmployeeProductDisplay(UserId);
            })
        }
    });
}

function AppointmentStatus(Id, Status) {
    var stchecked = "";
    $(".loader").show();
    if (Status == 1) {
        $(".loader").show();
        $.post("/Product/GetDataOfLanguage?Text='Do You want to Decline Request?'&id=2", function (data) {
            $(".loader").hide();
            var confirmDecline = confirm(data);
            if (confirmDecline) {
                $(".loader").show();
                Status = "D";
                if ($("#DeclineAppointmentId_" + Id).prop("checked")) {
                    stchecked = "Checked";
                }
                else {
                    stchecked = "Not";

                }
                $.post("/Reservation/StatusFinalChange?Id=" + Id + "&Status=" + Status + "&checkornot=" + stchecked + "", function () {
                    AllDataPls();
                });
            }
            else {
                $("#DeclineAppointmentId_" + Id).prop("checked", false);
            }
        });
    }
    else {
        $(".loader").show();
        $.post("/Product/GetDataOfLanguage?Text='Do You want to Close Appointment?'&id=3", function (data) {
            $(".loader").hide();
            var confirmClose = confirm(data);
            if (confirmClose) {
                $(".loader").show();
                Status = "C";
                if ($("#CompletedOrder_" + Id).prop("checked")) {
                    stchecked = "Checked";
                }
                else {
                    stchecked = "Not";

                }
                $.post("/Reservation/StatusFinalChange?Id=" + Id + "&Status=" + Status + "&checkornot=" + stchecked, function () {
                    AllDataPls();
                });
            }
            else {
                $("#CompletedOrder_" + Id).prop("checked", false);
            }
        });
    }
}
function TextLanguagePopUp(Link) {
    $.post(Link, function (data) {
        $(".loader").hide();
        var confirmClose = confirm(data);
        return confirmClose;
    });
}
function AllDataPls() {
    $(":checkbox").attr("autocomplete", "off");
    if (window.location.toString().indexOf('#') != -1) {
        var ab = window.location.hash.substr(1);
        if (ab == "openlistview") {
            location.reload();
        }
        else {
            window.location.hash = "openlistview";
            window.location.reload();
        }
    }
    else {
        window.location.hash = '#' + "openlistview";
        window.location.reload();
    }

    //$(".loader").hide();
}
function DataToBeBlock(BlockingDate, StatusText) {
    //Used For ManualReservation
    var St = 1;
    if (StatusText == undefined || StatusText == null) { St = 1; }
    else { St = 0; }
    var UserId = $("#EmployeeChoosed").val();
    $(".loader").show();
    var url = "";
    if (UserId != "" && UserId != undefined) {
        url = "/Home/BlockDateofCalander?Status=cmpEmp&PartMonYear=" + BlockingDate + "&UserId=" + UserId + "&StatusTxt=" + St;
    }
    else {
        url = "/Home/BlockDateofCalander?Status=cmp&PartMonYear=" + BlockingDate + "&StatusTxt=" + St;
    }
    $.ajax({
        url: url,
        cache: false,
        success: function (data) {
            var dataArray = data.split("~");
            $(".k-days span").removeAttr("style").removeAttr("disabled");
            $.each($(".k-days span[html!='']"), function () {
                var Tempdays = $(this);
                if ($.inArray(Tempdays.html(), dataArray) != -1) {
                    $(this).addClass("k-out-of-month");
                    Tempdays.addClass("k-out-of-month").attr('disabled', 'disabled').css('pointer-events', 'none');
                    this.style.color = "#ddd";
                    this.style.cursor = "default";
                }
            });
            $(".loader").hide();
        },
        error: function (request, status, error) { }
    });


}

function StatusOfBookingData(Id) {
    if ($("#PendingRquestAccept_" + Id).prop("checked")) {
        $(".loader").show();
        $.post("/Product/GetDataOfLanguage?Text='Do You want to Accept Booking?'&id=4", function (data) {
            $(".loader").hide();
            var confirmBooking = confirm(data);
            if (confirmBooking) {
                $(".loader").show();
                $.post("/Reservation/PendingDataReply?status=A &Id=" + Id, function () {
                    MoveTo();
                });
            }
            else {
                $("#PendingRquestAccept_" + Id).prop("checked", false);
            }
        });
    }
    //var confirmBooking = confirm("Do You want to Accept Booking?");
}

function StatusOfCalBookingData(Id, BookingDate) {

    if ($("#CalPendingRquestAccept_" + Id).prop("checked")) {
        $(".loader").show();
        $.post("/Product/GetDataOfLanguage?Text='Do You want to Accept Booking?'&id=4", function (data) {
            $(".loader").hide();
            var Calview = $("#calendarmonth .view-cal .active").prop("id");
            var view = "";
            if (Calview == "DayBTN") {
                view = "Day";
            }
            if (Calview == "WeekBTN") {
                view = "Week";
            }
            if (Calview == "MONTHBTN") {
                view = "Month";
            }
            var AllView = $("#EmployeesDaybtn .activebutton").prop("id");
            var AllviewStatus = "";
            if (AllView == "All" && view == "Day") {
                AllviewStatus = "All";
            }
            var confirmBooking = confirm(data);
            if (confirmBooking) {
                $(".loader").show();
                $.post("/Reservation/PendingDataReply?status=A &Id=" + Id + "&BookingDate=" + BookingDate + "&ViewStatus=" + view + "&AllView=" + AllviewStatus, function () {
                    if (window.location.toString().indexOf('#') != -1) {
                        var ab = window.location.hash.substr(1);
                        if (ab == "calendarmonth") {
                            $('#welcome').modal("hide");
                            location.reload();
                        }
                        else {
                            $('#welcome').modal("hide");
                            window.location.hash = 'calendarmonth';
                            window.location.reload();
                        }
                    }
                    else {
                        $('#welcome').modal("hide");
                        window.location.hash = '#calendarmonth';
                        window.location.reload();
                    }
                });
            }
            else {
                $("#CalPendingRquestAccept_" + Id).prop("checked", false);
            }
        });
    }
    //var confirmBooking = confirm("Do You want to Accept Booking?");
}

function RemoveBooking(Id) {
    //var RemoveBookingTemp = confirm("Do You want to Cancel Booking?");
    $(".loader").show();
    $.post("/Product/GetDataOfLanguage?Text='Do You want to Cancel Booking?'&id=5", function (data) {
        $(".loader").hide();
        var RemoveBookingTemp = confirm(data);
        if (RemoveBookingTemp) {
            $(".loader").show();
            $.post("/Reservation/PendingDataReply?status=D &Id=" + Id, function () {
                MoveTo();
            });
        }
        else {
            $("#PendingRquestCancel_" + Id).prop("checked", false);
        }
    });
}
function MoveTo() {
    window.location.href = "/Reservation/Reservation";
}
//function AddProductToDb() {
//    if (($('#Female').is(':checked') || $('#male').is(':checked'))) {
//        $("#Gendervalidation").html(' ');
//        var addprodvalidation = $("#AddProductData").validate().form();
//        if (addprodvalidation != false) {
//            var NullTempValue = "";
//            if ($("#EditCatgTypeId").val() != "") {
//                NullTempValue = "yes";
//            }
//            $('.loader').show();
//            var form = $("#AddProductData");
//            var url = form.attr("action");
//            var formData = form.serialize();
//            $.post(url, formData, function (data) {
//                $("#AddProductData")[0].reset();
//                $(".add-vacation-block").slideToggle();
//                if (NullTempValue == "yes") {
//                    $("#EditCatgTypeId").val("");
//                }
//                if (data == "yes") {
//                    ShowProductDetailAdd();
//                }
//                else {
//                    ShowProductDetail();
//                }
//            });
//        }
//    }
//    else {
//        $("#Gendervalidation").html("Please Select gender");
//        $("#AddProductData").validate().form();
//    }

//}
function ShowProductDetailAdd() {
    $.post("/Product/GetCommonLangauge?PageName=Product_Catalog&orderid=15", function (data) {
        $("#MsgAlert").html("<h2 class=\"modal-h4 text-center\">" + data + "</h2>");
        $('#myModal').modal("show");
    });
    $('.loader').hide();
}
function ShowProductDetail() {
    window.location.href = "/Product/Product_Catalog";
}
function DeleteProduct(CatgTypeId) {
    $(".loader").show();
    $.post("/Product/GetDataOfLanguage?Text='Do You want to delete Product?'&id=6", function (data) {
        $(".loader").hide();
        var confirmDelete = confirm(data);
        if (confirmDelete) {
            $(".loader").show();
            $.post("/Product/DeleteProduct?CatgTypeId=" + CatgTypeId, function () {
                ShowProductDetail();
            });
        }
    });
    //var confirmDelete = confirm("Do You want to delete Product?");
}
function EditProduct(CatgTypeId) {
    window.scrollTo(0, 0);
    $(".loader").show();
    $(".add-vacation-block").slideDown();
    $("#Female,#male").prop("checked", false);
    $.ajax({
        url: "/Product/EditProductDetail?CatgTypeId=" + CatgTypeId,
        cache: false,
        dataType: "json",
        success: function (Product) {
            $("#EditCatgTypeId").val(Product.ProductId);
            $("#productName").val(Product.ProductName);
            $("#ProductDuration").val(Product.Duration);
            $("#ProductPrice").val(Product.Amount);
            $("#ProductDescription").val(Product.SectionDesc);
            if (Product.Gender == "Both") {
                $("#Female,#male").prop("checked", true);
            }
            else if (Product.Gender == "male") {
                $("#male").prop("checked", true);
            }
            else if (Product.Gender == "Female") {

                $("#Female").prop("checked", true);
            }
            ShowHideCustomSettlementTextBox(Product.DefaultStatus, "CustomSettlementText", Product.CustomText, "C_Settlement_Text");
            $(".EventDisplayDrop").html(Product.GroupName + '<span class="caret"></span>');
            $("#SelectedPlateformGroup").val(Product.GroupIdByDefault);
            $("#SelectedGroup").val(Product.GroupIdByShop);
            $(".EventGroupDisplay").html(Product.DefaultGroupName + '<span class="caret"></span>');
            if (Product.InsuranceId > 0) {
                $("#SelectedInsurance").val(Product.InsuranceId);
                $("#InsuranceDropDown").val(Product.Tarif_Number + "  " + Product.Settlement_Number + "  " + Product.Insurance);
            }

            if (Product.vat == "0") {
                Product.vat = "0.0"
            }
            if (Product.vat != "" && Product.vat != null) {
                $("#SelectedVat").val(Product.vat);
                $(".VatTextDisplay").html(Product.vat + '<span class="caret"></span>');
            }
            $(".loader").hide();
        },
        error: function (request, status, error) { }
    });
}

function ChooseEmployee() {
    var ChkMsg = "";
    $.each($(".ProductDiv input"), function () {
        if ($(this).prop("checked")) {
            ChkMsg = "2";
        }
    });
    if (ChkMsg == "2") {
        $('.loader').show();
        var form = $("#ClientProductData");
        var formProductData = form.serialize();
        var url = form.attr("action");
        $.post(url, formProductData, function (data) {
            if (data == "GoToEmployee") {
                PostToChooseEmployee();
            }
            if (data == "GoToLogin") {
                redirecttoLogin();
            }
        });
    }
    else {
        $(".loader").show();
        $.post("/Product/GetDataOfLanguage?Text='Please select Product'&id=7", function (data) {
            $(".loader").hide();
            $("#MsgAlert").html("<h2 class=\"modal-h4 text-center\">" + data + "</h2>");
            $('#myModal').modal("show");
        });

    }
}
function ChooseEmployeeOrder(Status, Url) {
    var ChkMsg = "";
    $.each($(".ProductDiv input"), function () {
        if ($(this).prop("checked")) {
            ChkMsg = "2";
        }
    });
    if (ChkMsg == "2") {
        $('.loader').show();
        var form = $("#ClientProductData");
        var formProductData = form.serialize();
        var url = form.attr("action");
        $.post(url, formProductData, function (data) {
            window.location.href = "/BookOrder/ChooseEmployee?Id=" + data + "&Status=" + Status + "&Url=" + Url;
        });
    }
    else {
        $(".loader").show();
        $.post("/Product/GetDataOfLanguage?Text='Please select Product'&id=7", function (data) {
            $(".loader").hide();
            $("#MsgAlert").html("<h2 class=\"modal-h4 text-center\">" + data + "</h2>");
            $('#myModal').modal("show");
        });

    }
}
function PostToChooseEmployee() {
    window.location.href = "/Home/ChooseEmployee";
}
function redirecttoLogin() {
    window.location.href = "/Login/Login";
}
function DisplayDateTimeDetail(UserId, StatusText) {
    var St = 1;
    if (StatusText == undefined || StatusText == null) { St = 1; }
    else { St = 0; }
    $(".loader").show();
    $(".k-calendar .k-days .k-selected").removeClass("k-selected");
    $("#DefaultUser").removeClass("active");
    $("#AllImage div").removeClass("active");
    $("#" + UserId).addClass("active");
    $("#EmployeeChoosed").val(UserId);
    $("#TimeSlotDataForm").hide();
    $("#DateTimeDisplayEmployee").show();
    //var PartMonth = $(".k-caption").prop("id");
    var PartMonth = moment($.trim($(".k-caption").html().split(',')[0]), 'MMM').lang('en').format("MMMM") + ", " + $.trim($(".k-caption").html().split(',')[1]);
    var URLPATH = "/Home/BlockDateofCalander?Status=cmpEmp&PartMonYear=" + PartMonth + " &UserId=" + UserId + "&StatusTxt=" + St;
    $.ajax({
        url: URLPATH,
        beforeSend: function () {
            if (!$(".loader").is(":visible"))
                $(".loader").show();
        },
        type: 'POST',
        async: true,
        success: function (data) {
            var dataArray = data.split("~");
            $(".k-days span").removeAttr("style").removeAttr("disabled");
            $(dataArray).each(function (index, value) {
                $.each($(".k-days .k-active"), function () {
                    if (($(this).html()) == value) {
                        $(this).attr('disabled', 'disabled');
                        this.style.cursor = "default";
                        $(this).css('pointer-events', 'none');
                        $(this).addClass("k-out-of-month");
                        this.style.color = "#ddd";
                    }
                });
            });
            $("#UserIdOrder").val(UserId);
            $(".loader").hide();
        },
        complete: function () {
            $(".loader").hide();
        },
        error: function (request, status, error) { $(".loader").hide(); },
        cache: false,
        contentType: false,
        processData: false
    });

    //$.post("/Home/BlockDateofCalander?Status=cmpEmp&PartMonYear=" + PartMonth + " &UserId=" + UserId + "&StatusTxt=" + St, function (data) {
    //    var dataArray = data.split("~");
    //    $(".k-days span").removeAttr("style").removeAttr("disabled");
    //    $(dataArray).each(function (index, value) {
    //        $.each($(".k-days .k-active"), function () {
    //            if (($(this).html()) == value) {
    //                //$(this).off("click");
    //                $(this).attr('disabled', 'disabled');
    //                this.style.cursor = "default";
    //                $(this).css('pointer-events', 'none');
    //                $(this).addClass("k-out-of-month");
    //                this.style.color = "#ddd";
    //            }
    //        });
    //    });
    //$("#UserIdOrder").val(UserId);
    //$(".loader").hide();
    //});

}
function AutoSelect(UserId, StatusText) {
    var St = 1;
    if (StatusText == undefined || StatusText == null) { St = 1; }
    else { St = 0; }
    $(".loader").show();
    var DateOld = moment($.trim($(".k-caption").html().split(',')[0]), 'MMM').lang('en').format("MMMM") + ", " + $.trim($(".k-caption").html().split(',')[1]);
    $.post("/Home/GetAllLangDays", function (DaysAdded) {
        var DaysSep = DaysAdded.BlockData;
        var count = 0;
        $.each($(".k-header span"), function () {
            if (count == 0) { $(this).html(DaysSep[6]); }
            else { $(this).html(DaysSep[count - 1]); }
            count = count + 1;
        });
    });
    $(".k-calendar .k-days .k-selected").removeClass("k-selected");
    $("#EmployeeChoosed").val(UserId);
    $("#TimeSlotDataForm").hide();
    $("#DateTimeDisplayEmployee").show();
    //Ankit
    var PartMonth = DateOld;
    $.post("/Home/BlockDateofCalander?Status=cmpEmp&PartMonYear=" + PartMonth + " &UserId=" + UserId + "&StatusTxt=" + St, function (data) {
        var dataArray = data.split("~");
        $(".k-days span").removeAttr("style").removeAttr("disabled");
        $(dataArray).each(function (index, value) {
            $.each($(".k-days span"), function () {
                if (($.trim($(this).html())) == value) {
                    $(this).addClass("k-out-of-month").attr('disabled', 'disabled').css('pointer-events', 'none');
                    this.style.cursor = "default";
                    this.style.color = "#ddd";
                }
            });
        });
        $(".loader").hide();
    });
    $("#UserIdOrder").val(UserId);

}
function GetEditEmployee(UserIdValue) {
    try {
        $.ajax({
            url: "/Employee/GetEmployeeData?UserId=" + UserIdValue,
            cache: false,
            dataType: "json",
            success: function (data) {
                var UserIdTemp = data.UserId;
                $("#UserId").val(UserIdTemp);
                $('#WeekUserId').val(UserIdTemp);
                $('#TimeSlotUserId').val(UserIdTemp);
                $('#ProductUserId').val(UserIdTemp);
                $("#VacationId").val(UserIdTemp);
                $("#EmployeeSubmittedOrNot").val("Yes");
                if (data.Title != "") {
                    $('.MenuDrop').html(data.Title + "<span class=\"caret\"></span>");
                    $("#Gender").val(data.Title);
                }
                GetDaysEdited(UserIdTemp);
            },
            error: function (request, status, error) { }
        });
    }
    catch (err) {
        window.location.href = "/Employee/EmployeeMaster";
    }
}
function BookMyOrder(ReservationId) {
    $(".loader").show();
    var Comments = $("#Comments").val();
    $.post("/Home/BookMyOrder?Comments=" + Comments + "&ReservationId=" + ReservationId, function (Result) {
        if (Result == "error")
            window.location.href = "/Home/ChooseYourProduct";
        else
            window.location.href = "/Home/ThanksMessage";
    });
}
function GetDaysEdited(UserId) {
    $.ajax({
        url: "/Employee/GetDetailEmployeeData?UserId=" + UserId,
        type: 'POST',
        dataType: "json",
        async: true,
        success: function (data) {
            if (data.BlockData != null && data.BlockData != "" && data.BlockData != "undefined") {
                var dataArray = data.BlockData;
                jQuery.each(dataArray, function (i, val) {
                    $("#" + val).attr('checked', true);
                    $(".week-schedule-content #1" + val).removeClass("inactive");
                    $(".week-schedule-content #2" + val).removeClass("inactive");
                    $(".week-schedule-content #3" + val).removeClass("inactive");
                    $(".week-schedule-content #4" + val).removeClass("inactive");
                    $("#1" + val).removeAttr("disabled");
                    $("#2" + val).removeAttr("disabled");
                    $("#3" + val).removeAttr("disabled");
                    $("#4" + val).removeAttr("disabled");
                    $("#1" + val).addClass("required");
                    $("#2" + val).addClass("required");
                    $("#3" + val).addClass("required");
                    $("#4" + val).addClass("required");
                });
                $("#WeekScheduleAdded").val("Yes");
            }
            GetSlotTimeEditDetail(UserId);
        },
        cache: false,
    });
    return false;
}
function GetSlotTimeEditDetail(UserId) {
    $.post("/Employee/GetEmployeeTimeSlot?UserId=" + UserId, function (data) {
        if (data != "" || data != null) {
            $("#" + data).attr('checked', true);
            $("#timeSlotAdded").val("Yes");
        }
    });
}


//function editWeekDivShow(itemId) {
//    var url = "/Employee/displayEditWeek?CatTid=" + itemId;
//    $("#EditWeek").show();
//    $.post(url, function (data) {
//        var dataArray = data.split("~");
//        $("#EditCatgDesc").val(dataArray[0]);
//        $("#EditCatgType").val(dataArray[1]);
//        $("#EditItemId").val(itemId);
//    });
//}
function editWeekDivShow(itemId) {
    $("#EditWeek").show();
    $.ajax({
        url: "/Employee/displayEditWeek?CatTid=" + itemId,
        type: 'POST',
        dataType: "json",
        async: true,
        success: function (data) {
            $("#EditCatgDesc").val(data.CatgDesc);
            $("#EditCatgType").val(data.CatgType);
            $("#EditItemId").val(itemId);
        },
        cache: false,
    });
    return false;
}
function closeWeekDiv() {
    $("#EditWeek").hide();
}
function Updateweek() {
    var form = $("#WeekUpdateForm");
    var url = form.attr("action");
    var formData = form.serialize();
    $.post(url, formData, function (data) {
        $("#WeekUpdateForm")[0].reset();
        WeekDetailDisplayingEdit();
    });
}
function WeekDetailDisplayingEdit() {
    $("#EditWeek").hide();
    var url = "/Employee/WeekDetail";
    $.post(url, function (data) {
        $("#WeekDetailDisplay").html(data);
    });
}
function showAll() {
    window.location.href = "/Employee/AddWeek";
}
function deleteWeekDivShow(itemId) {
    var url = "/Employee/DeleteWeek?CatTid=" + itemId;
    $.post(url, function () {
        showAll();
    });
}
function ActivateDiv(itemId) {
    var url = "/Employee/ActivateWeek?CatTid=" + itemId;
    $.post(url, function () {
        showAll();
    });
}
//function DisplayEditPeriod(schedulerId) {
//    $("#TableDetail").hide();
//    $("#EditDiv").show();
//    var url = "/Employee/DisplayEditPeriod?SchedulerId=" + schedulerId;
//    $.post(url, function (data) {
//        var dataArray = data.split("~");
//        $("#WeekDayEdit").val(dataArray[0]);
//        $("#EditStartTime").val(dataArray[1]);
//        $("#EditEndTime").val(dataArray[2]);
//        $("#SchedulerId").val(dataArray[3]);
//    });
//}
function DisplayEditPeriod(schedulerId) {
    $("#TableDetail").hide();
    $("#EditDiv").show();
    $.ajax({
        url: "/Employee/DisplayEditPeriod?SchedulerId=" + schedulerId,
        type: 'POST',
        dataType: "json",
        async: true,
        success: function (Data) {
            $("#WeekDayEdit").val(Data.WeekDay);
            $("#EditStartTime").val(Data.StartTime);
            $("#EditEndTime").val(Data.EndTime);
            $("#SchedulerId").val(Data.SchedulerId);
        },
        cache: false,
    });
    return false;
}
function EditPeriodCancel() {
    $("#TableDetail").show();
    $("#EditDiv").hide();
}
function EditPeriod() {
    var form = $("#PeriodUpdateForm");
    var url = form.attr("action");
    var formData = form.serialize();
    $.post(url, formData, function () {
        $("#PeriodUpdateForm")[0].reset();
        window.location.href = "/Employee/AddWeekPeriod";
    });
}
function DeleteWeekPeriod(SchedulerId) {
    var url = "/Employee/DeletePeriod?SchedulerId=" + SchedulerId;
    $.post(url, function () {
        window.location.href = "/Employee/AddWeekPeriod";
    });
}
function ReplicateDays(SchedulerId) {
    var url = "/Employee/ReplicateDays?SchedulerId=" + SchedulerId;
    $.post(url, function () {
        window.location.href = "/Employee/AddWeekPeriod";
    });

}
function setImage(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            $('#UserImage').attr('src', e.target.result);
            $('#DeletePic').show();
            $("#Answer2").val("");
        }
        reader.readAsDataURL(input.files[0]);
        //$("#DeletePic").show();

    }
}
function setImageShop(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            $('#UserImageshop').attr('src', e.target.result);
            $('#DeletePicShop').show();
            $("#TempImage").val("");
        }
        reader.readAsDataURL(input.files[0]);
        //$("#DeletePic").show();

    }
}
function btnDaysAdd() {
    // var dtValidate = $("#FormProfile").validate().form();
    var Length = $(".time_section li.time_section-li input:text").filter(function () { return this.value == "" && $(this).attr("disabled") != "disabled" }).length;
    if (Length == 0) {
        $('.loader').show();
        if ($(".ChkWeek input:checkbox:checked").length > 0) {
            if ($("#FormProfile #WeekUserId").val() != "" && $("#FormProfile #WeekUserId").val() != 0) {
                var formData = new FormData($("#FormProfile")[0]);
                $.ajax({
                    url: $("#FormProfile").attr("action"),
                    beforeSend: function () {
                        $(".loader").show();
                    },
                    type: 'POST',
                    dataType: "json",
                    data: formData,
                    async: true,
                    success: function (data) {
                        if (data.Error == "1") {
                            $('.loader').hide();
                            $("#MsgAlert").html("<h2 class=\"modal-h4 text-center\">" + data.ErrorMessage + "</h2>");
                            $('#myModal').modal("show");

                        }
                        else {
                            $("#WeekScheduleAdded").val("Yes");
                            $("#DaysAdd").text("Update");
                            GetEditEmployee(data.UserId);
                            $('.loader').hide();
                        }
                    },
                    cache: false,
                    contentType: false,
                    processData: false
                });
                return false;

            }
            else {
                $.post("/Product/GetDataOfLanguage?Text='Please Enter Employee Information'&id=11", function (data) {
                    $('.loader').hide();
                    $("#MsgAlert").html("<h2 class=\"modal-h4 text-center\">" + data + "</h2>");
                    $('#myModal').modal("show");
                });
            }
        }
        else {
            $.post("/Product/GetDataOfLanguage?Text='Please Enter Employee Information'&id=60", function (data) {
                $('.loader').hide();
                $("#MsgAlert").html("<h2 class=\"modal-h4 text-center\">" + data + "</h2>");
                $('#myModal').modal("show");
            });
        }
    }
    else
        $(".time_section li.time_section-li input:text").filter(function () { return this.value == "" && $(this).attr("disabled") != "disabled" }).css("border-color", 'red').delay(2000).queue(function () { $(".time_section li.time_section-li input:text").filter(function () { return this.value == "" && $(this).attr("disabled") != "disabled" }).removeAttr('style').clearQueue() });
}
function DeletePicture() {
    $("#FileProfile").val('');
    //$('#UserImage').attr('src', '~/images/defaultuser.png');
    document.getElementById('UserImage').src = "/images/defaultuser.png";
    $("#Answer2").val("");

    $("#DeletePic").hide();

}
function DeletePictureShop() {

    $("#ImageLogo").val('');

    //$('#UserImage').attr('src', '~/images/defaultuser.png');
    $("#TempImage").val("Delete");
    document.getElementById('UserImageshop').src = "/images/shoplogo.png";


    $("#DeletePicShop").hide();

}
function AddTimeSlotForEmployee() {
    $('.loader').show();
    if ($("#EmployeeSubmittedOrNot").val() == "Yes") {
        if ($("#WeekScheduleAdded").val() == "Yes") {
            var CheckBoxSelectd = 0;
            $.each($(".time-slot-box input"), function () {
                if ($(this).prop("checked")) {
                    CheckBoxSelectd = CheckBoxSelectd + 1;
                }
            });
            if (CheckBoxSelectd > 1) {
                $.post("/Product/GetDataOfLanguage?Text='please do not select time slot more than one'&id=12", function (data) {
                    $('.loader').hide();
                    $("#MsgAlert").html("<h2 class=\"modal-h4 text-center\">" + data + "</h2>");
                    $('#myModal').modal("show");
                });

            }
            else {
                var form1 = $("#FormTimeSlot");
                var url = form1.attr("action");
                var formData = form1.serialize();
                $.post(url, formData, function (Data) {
                    $("#timeSlotAdded").val("Yes");
                    $("#btnTimeSlot").val("Update");
                    $('.loader').hide();
                });
            }

        }
        else {
            $.post("/Product/GetDataOfLanguage?Text='Please Add Weeks'&id=13", function (data) {
                $('.loader').hide();
                $("#MsgAlert").html("<h2 class=\"modal-h4 text-center\">" + data + "</h2>");
                $('#myModal').modal("show");
            });

        }

    }
    else {
        $.post("/Product/GetDataOfLanguage?Text='Please Enter Employee Information'&id=11", function (data) {
            $('.loader').hide();
            $("#MsgAlert").html("<h2 class=\"modal-h4 text-center\">" + data + "</h2>");
            $('#myModal').modal("show");
        });
        //alert("Please Register Employee");
    }
}
function LoaderShowing() {
    $(".loader").show();
}
function SubmitEmployee() {
    var FormEmployee = $("#EmployeeForm").validate().form();
    if (FormEmployee != false) {
        var formData = new FormData($("#EmployeeForm")[0]);
        $.ajax({
            url: $("#EmployeeForm").attr("action"),
            beforeSend: function () {
                $(".loader").show();
            },
            type: 'POST',
            data: formData,
            async: true,
            success: function (data) {
                if (data > "0") {
                    $("#UserId").val(data);
                    $('#WeekUserId').val(data);
                    $('#TimeSlotUserId').val(data);
                    $('#ProductUserId').val(data);
                    $("#VacationId").val(data);
                    $("#EmployeeSubmittedOrNot").val("Yes");
                    EmployeeProductDisplay(data);
                }
                if (data == "1")
                    window.location.href = "/Employee/CreateEmployee";
            },
            cache: false,
            contentType: false,
            processData: false
        });
        return false;
    }
    else {
        $('.loader').hide();
    }

}
function EmployeeProductDisplay(UserId) {
    window.location.href = "/Employee/CreateEmployee?UserId=" + UserId;
}
function ProductAssigning() {
    $('.loader').show();
    var CheckBoxChk = 0;
    if ($("#UserId").val() != "") {
        $.each($('.AssignEmpProduct input[type="checkbox"]'), function () {
            if ($(this).prop('checked')) {
                CheckBoxChk = 1;
            }
        });
        if (CheckBoxChk == 1) {
            var UserId = $("#ProductUserId").val();
            if (UserId == "") {
                UserId = $("#UserId").val();
            }
            if (UserId != "" || $("#UserId").val() != "") {
                UserId = $("#UserId").val();
                $(".loader").show();
                var form = $("#FormProduct");
                var url = form.attr("action");
                var formData = form.serialize();
                $.post(url, formData, function () {
                    $("#AllProduct").hide();
                    $("#DisplayCopy").hide();
                    $("#ProductAddedOrNot").val("Yes");
                    $("#btnAllProduct").val("Update");
                    EmployeeProductDisplay(UserId);
                });
            }
            else {
                $.post("/Product/GetDataOfLanguage?Text='Still You are Not Logged In'&id=14", function (data) {
                    $('.loader').hide();
                    $("#MsgAlert").html("<h2 class=\"modal-h4 text-center\">" + data + "</h2>");
                    $('#myModal').modal("show");
                });
            }
        }
        else {
            $.post("/Product/GetDataOfLanguage?Text='Still You are Not Logged In'&id=7", function (data) {
                $('.loader').hide();
                $("#MsgAlert").html("<h2 class=\"modal-h4 text-center\">" + data + "</h2>");
                $('#myModal').modal("show");
            });
        }
    }
    else {
        $.post("/Product/GetDataOfLanguage?Text='Please Register Employee'&id=15", function (data) {
            $('.loader').hide();
            $("#MsgAlert").html("<h2 class=\"modal-h4 text-center\">" + data + "</h2>");
            $('#myModal').modal("show");
        });
    }
}
//function VacationAdding() {
//    var ValidationofForm = $("#VacationForm").validate().form();
//    if (ValidationofForm != false) {
//        $('.loader').show();
//        if ($("#EmployeeSubmittedOrNot").val() == "Yes") {
//            var UserId = $("#VacationId").val();
//            var url = $("#VacationForm").attr("action");
//            var formData = new FormData($("#VacationForm")[0]);
//            $.ajax({
//                url: url,
//                type: 'POST',
//                data: formData,
//                async: false,
//                success: function (Result) {
//                    if (Result.split("~")[0] == "ErrorLeaves") {
//                        $.post("/Shop/RestrictionAddLeaves?Status=EmployeeLeave" + "&Startdate=" + Result.split("~")[1] + "" + "&EndDate=" + Result.split("~")[2] + "" + "&Empuserid=" + Result.split("~")[3], function (data1) {
//                            $("#WelcomeAlert").html("<h1 class=\"text-center margin-bottom30\"><span>CLICK-AND-GO</span></h1><p class=\"title15 text-center margin-bottom30\">" + data1 + "</p>");
//                            $('#welcome').modal("show");
//                        });
//                    }
//                    $(".add-vacation-block").slideToggle();
//                    $("#VacationForm")[0].reset();
//                    VacationDetails(UserId)
//                },
//                cache: false,
//                contentType: false,
//                processData: false
//            });
//            return false;
//        }
//        else {

//            $.post("/Product/GetDataOfLanguage?Text='Please Register Employee'&id=15", function (data) {
//                $('.loader').hide();
//                $("#MsgAlert").html("<h2 class=\"modal-h4 text-center\">" + data + "</h2>");
//                $('#myModal').modal("show");
//            });
//        }
//    }
//}
function VacationAdding() {
    var ValidationofForm = $("#VacationForm").validate().form();
    if (ValidationofForm != false) {
        $('.loader').show();
        if ($("#EmployeeSubmittedOrNot").val() == "Yes") {
            var UserId = $("#VacationId").val();
            var url = $("#VacationForm").attr("action");
            var formData = new FormData($("#VacationForm")[0]);
            $.ajax({
                url: url,
                type: 'POST',
                data: formData,
                dataType: "json",
                async: false,
                success: function (Result) {
                    if (Result.Status == "ErrorLeaves") {
                        $.post("/Shop/RestrictionAddLeaves?Status=EmployeeLeave" + "&Startdate=" + Result.StartDate + "" + "&EndDate=" + Result.EndDate + "" + "&Empuserid=" + Result.UserId, function (data1) {
                            $("#WelcomeAlert").html("<h1 class=\"text-center margin-bottom30\"><span>CLICK-AND-GO</span></h1><p class=\"title15 text-center margin-bottom30\">" + data1 + "</p>");
                            $('#welcome').modal("show");
                        });
                    }
                    $(".add-vacation-block").slideToggle();
                    $("#VacationForm")[0].reset();
                    VacationDetails(UserId)
                },
                cache: false,
                contentType: false,
                processData: false
            });
            return false;
        }
        else {

            $.post("/Product/GetDataOfLanguage?Text='Please Register Employee'&id=15", function (data) {
                $('.loader').hide();
                $("#MsgAlert").html("<h2 class=\"modal-h4 text-center\">" + data + "</h2>");
                $('#myModal').modal("show");
            });
        }
    }
}
function VacationDetails(UserId) {
    $.post("/Employee/VacationDetail?UserId=" + UserId, function (data) {
        $("#VacationTableDetail").html(data);
        $('.loader').hide();
    });
}
function Logout() {

    window.location.href = "/Employee/Logout";
}
/*Customer Section*/
function BlackEditCustomer(UserId) {
    $("#blacklist").removeClass("in active collapsed");
    var Status = "L";
    EditCustomerDetail(UserId, Status);
}
//function EditCustomerDetail(UserId, Status) {
//    if (Status != "L") {
//        Status = "A";
//    }
//    $("#status").val(Status);
//    if (Status == 'L') {
//        $.post("/Product/GetCommonLangauge?PageName=Customer_Tab&orderid=6", function (data) {
//            $("#NewCustomerLI a").html(data);
//            $("#Cancelnewcustomer").hide();
//            $("#CancelnewcustomerBackToBlackList").show();
//            $("#CancelnewcustomerBackToViewList").hide();
//        });
//    }
//    else {
//        $.post("/Product/GetCommonLangauge?PageName=Customer_Tab&orderid=6", function (data) {
//            $("#NewCustomerLI a").html(data);
//            $("#CancelnewcustomerBackToViewList").show();
//            $("#CancelnewcustomerBackToBlackList").hide();
//            $("#Cancelnewcustomer").hide();
//        });
//    }
//    $(".loader").show();
//    //Changes Required when structure change
//    if ($(window).width() < 768) {
//        $("#newcustomer-collapse").parent().find("a:first").click();
//    }
//    else {
//        $.each($("#myTab li"), function () {
//            $(this).removeClass("active");
//        });
//        $("#NewCustomerLI").addClass("active");
//        $("#listview").removeClass("in active");
//        $("#newcustomer").addClass("in active");
//    }

//    $.ajax({
//        url: "/Customer/EditCustomerDisplay?UserId=" + UserId + "&Status=" + Status + "",
//        cache: false,
//        dataType: "json",
//        success: function (data) {
//            $("#NwPhoneNo").val(data.PhoneNo);
//            $("#NWEditUserId").val(data.UserId);
//            $("#NWFirstName").val(data.FirstName);
//            $("#NWLastName").val(data.LastName);
//            $("#NWEmail").val(data.EmailId);
//            $("#NWPostal").val(data.PostalCode);
//            $("#NWCITY").val(data.City);
//            $("#NWAddress").val(data.Address);
//            $(".NwCust").html(data.Gender + "<span class=\"caret\"></span>");
//            $("#NWPassword").val(data.Password);
//            $("#NWCFPassword").val(data.Password);
//            $("#NWTitle").val(data.Gender);
//            if (data.DOB != "01/01/0001" && data.DOB != "01-01-0001") {
//                $("#NWDOB").val(data.DOB);
//            }
//            else {
//                $("#NWDOB").val("");
//            }
//            var CustCreatedOn = data.CreatedOn;
//            $("#EmailConfirm").prop("checked", false);
//            $("#SmsConfirm").prop("checked", false);
//            $("#EmailRemain").prop("checked", false);
//            $("#SmsRemain").prop("checked", false);
//            $("#blackListValue").prop("checked", false);
//            var SMSSERVICE = data.SMSService;
//            var EMAILSERVICE = data.EmailService;
//            if (SMSSERVICE != 0) {
//                if (SMSSERVICE == 1) {
//                    $("#SmsRemain").prop("checked", true);
//                }
//                else if (SMSSERVICE == 2) {
//                    $("#SmsRemain").prop("checked", true);
//                    $("#SmsConfirm").prop("checked", true);
//                }
//                else {
//                    $("#SmsConfirm").prop("checked", true);
//                }
//            }
//            if (EMAILSERVICE != 0) {
//                if (EMAILSERVICE == 1) {
//                    $("#EmailRemain").prop("checked", true);
//                }
//                else if (EMAILSERVICE == 2) {
//                    $("#EmailRemain").prop("checked", true);
//                    $("#EmailConfirm").prop("checked", true);
//                }
//                else {
//                    $("#EmailConfirm").prop("checked", true);
//                }
//            }
//            DataFurtherDisplay(UserId, CustCreatedOn, Status);
//        },
//        error: function (request, status, error) { }
//    });
//}s
function EditCustomerDetail(UserId, Status) {;

    var url = "/Customer/NewCustomer?UserId=" + UserId + "&Status=" + Status;
    //Changes Required when structure change
    if ($(window).width() < 768) {
        $("#newcustomer-collapse").parent().find("a:first").click();
    }
    else {
        $.each($("#myTab li"), function () {
            $(this).removeClass("active");
        });
        $("#NewCustomerLI").addClass("active");
        $("#listview").removeClass("in active");
        $("#newcustomer").addClass("in active");
    }
    $(".loader").show();
    $.ajax({
        url: url,
        type: 'POST',
        async: true,
        success: function (result) {
            $("#newcustomer").html(result);
            if (Status != "L") {
                Status = "A";
            }
            $("#status").val(Status);
            if (Status == 'L') {
                $.post("/Product/GetCommonLangauge?PageName=Customer_Tab&orderid=6", function (data) {
                    $("#NewCustomerLI a").html(data);
                    $("#Cancelnewcustomer").hide();
                    $("#CancelnewcustomerBackToBlackList").show();
                    $("#CancelnewcustomerBackToViewList").hide();
                });
            }
            else {
                $.post("/Product/GetCommonLangauge?PageName=Customer_Tab&orderid=6", function (data) {
                    $("#NewCustomerLI a").html(data);
                    $("#CancelnewcustomerBackToViewList").show();
                    $("#CancelnewcustomerBackToBlackList").hide();
                    $("#Cancelnewcustomer").hide();
                });
            }          
            $(".loader").hide();
        },
        cache: false,
    });

}
function DataFurtherDisplay(UserId, CustCreatedOn, Status) {
    $.post("/Customer/DisplayCustomerHistory?UserId=" + UserId, function (data) {
        $("#HistoryInfoCustomer").html(data);
        $.post("/Product/GetCommonLangauge?PageName=Customer_New_Customer&orderid=16", function (data) {
            $("#custSince").html(data + ": " + CustCreatedOn);
        });

        if (Status == "L") {
            $("#blackListValue").prop("checked", true);
        }
        $(".loader").hide();

    });
}
function CustomerDelete(UserId, tyi) {
    $.post("/Product/GetDataOfLanguage?Text='Do You Want To Delete Customer?'&id=16", function (data) {
        var ConfirmCustDelete = confirm(data);
        if (ConfirmCustDelete) {
            $(".loader").show();
            $.post("/Customer/DeleteCustomer?UserId=" + UserId, function () {
                if (tyi == 5) {
                    BlackListShowData(tyi);
                }
                else {
                    showListdataAfter();
                }
            });
        }
    });

}
function EditLeave(LeaveId) {
    $(".loader").show();
    $.getJSON("/Employee/EditLeave?LeaveId=" + LeaveId, function (data) {
        $(".add-vacation-block").slideToggle();
        $("#EditVacationId").val(LeaveId);
        $("#VacationDescEdit").val(data.HolidayName);
        $("#StartDateEdit").val(data.StartDate);
        $("#EndDateEdit").val(data.EndDate);
        $(".loader").hide();
    });
}
function DeleteLeave(LeaveId, UserId) {
    $.post("/Product/GetDataOfLanguage?Text='Do you want To Delete Leave?'&id=17", function (data) {
        var delLeave = confirm(data);
        if (delLeave) {
            $(".loader").show();
            $.post("/Employee/DeleteLeave?LeaveId=" + LeaveId, function () {
                VacationDetails(UserId);
                $(".loader").hide();
            });
        }
    });
}
function EditText(SolutionId) {
    $(".loader").show();
    $.ajax({
        url: "/Shop/EditTextDisplay?SolutionId=" + SolutionId,
        cache: false,
        dataType: "json",
        success: function (data) {
            window.scrollTo(0, 0);
            $(".BlockText").slideToggle();
            $(".EventDisplayDrop").html($.trim($("[id='" + data.CatgDesc + "']").html()) + '<span class="caret"></span>');
            $("#EditUserId").val(data.SolutionId);
            $("#SelectedEvent").val(data.CatgDesc);
            if (data.Media == "Email") {
                $("#EmailId").prop("checked", true);
                $("#SMSFIELD").prop("checked", false);
            }
            if (data.Media == "SMS") {
                $("#SMSFIELD").prop("checked", true);
                $("#EmailId").prop("checked", false);
            }
            $("#MsgArea").val(data.SectionDesc);
            $(".loader").hide();
        },
        error: function (request, status, error) { }
    });
}
function DeleteText(SolutionId) {
    $.post("/Product/GetDataOfLanguage?Text='Do you want Delete Text?'&id=18", function (data) {
        var confirmText = confirm(data)
        if (confirmText) {
            $(".loader").show();
            $.post("/Shop/DeleteText?SolutionId=" + SolutionId, function () {
                var tiq = 9;
                showAllTextValue(tiq);
            });
        }
    });
    //window.location.href = "/Shop/Shop";
}
function BlackListShowData(tyi) {
    $.post("/Customer/BlackList", function (data) {
        $("#blacklist").html(data);
        if (tyi != 5) {
            showListdataAfter();
        }
        else {
            $(".loader").hide();
        }
    });
}
function showListdataAfter() {
    $.post("/Customer/ListView", function (data) {
        if ($(window).width() < 768) {
            $("#listview-collapse").html(data);
        }
        else {
            $("#listview").html(data);
        }

        $(".loader").hide();
    });
}
//function AddShopLeave() {
//    var data = $("#HolidayForm").validate().form();    
//    if (data != false) {
//        $(".loader").show();
//        var form = $("#HolidayForm");
//        var url = form.attr("action");
//        alert(url);
//        var formData = form.serialize();
//        $.post(url, formData, function (Result) {
//            if (Result.split("~")[0] == "ErrorLeaves") {
//                $.post("/Shop/RestrictionAddLeaves?Status=ShopLeave" + "&Startdate=" + Result.split("~")[1] + "" + "&EndDate=" + Result.split("~")[2] + "", function (data1) {
//                    $("#WelcomeAlert").html("<h1 class=\"text-center margin-bottom30\"><span>CLICK-AND-GO</span></h1><p class=\"title15 text-center margin-bottom30\">" + data1 + "</p>");
//                    $('#welcome').modal("show");
//                });
//            }
//            $("#HolidayForm")[0].reset()
//            $("#EditLeaveId").val("");
//            $("#HolidayType").val("");
//            DisplayHolidayview();

//        });
//    }

//}
function AddShopLeave() {
    $(".loader").show();
    var data = $("#HolidayForm").validate().form();
    if (data != false) {
        var url = $("#HolidayForm").attr("action");
        var formData = new FormData($("#HolidayForm")[0]);
        $.ajax({
            url: url,
            beforeSend: function () {
                if (!$(".loader").is(":visible"))
                    $(".loader").show();
            },
            type: 'POST',
            data: formData,
            dataType: "json",
            async: false,
            success: function (Result) {
                if (Result.Status == "ErrorLeaves") {
                    $.post("/Shop/RestrictionAddLeaves?Status=ShopLeave" + "&Startdate=" + Result.StartDate + "" + "&EndDate=" + Result.EndDate + "", function (data1) {
                        $("#WelcomeAlert").html("<h1 class=\"text-center margin-bottom30\"><span>CLICK-AND-GO</span></h1><p class=\"title15 text-center margin-bottom30\">" + data1 + "</p>");
                        $('#welcome').modal("show");
                    });
                }
                $("#HolidayForm")[0].reset()
                $("#EditLeaveId").val("");
                $("#HolidayType").val("");
                DisplayHolidayview();
            },
            complete: function () {
                $(".loader").hide();
            },
            error: function (request, status, error) { $(".loader").hide(); },
            cache: false,
            contentType: false,
            processData: false
        });
        return false;
    }
    else {
        $(".loader").hide();
    }
}
function DeleteShopLeave(LeaveId) {
    $.post("/Product/GetDataOfLanguage?Text='Do You want to Delete Shop Holiday?'&id=19", function (data) {
        var confirmLeave = confirm(data);
        if (confirmLeave) {
            $(".loader").show();
            $.post("/Shop/DeleteShop?LeaveId=" + LeaveId, function () {
                var Tyoh = 3;
                DisplayHolidayview(Tyoh);
            });
        }
    });
}
function DisplayHolidayview(Tyoh) {
    $.post("/Shop/HolidayDetail", function (data) {
        $("#HolidayDetailId").html(data);
        if (Tyoh != 3) {
            $("#HolidayAddShop").slideToggle();
        }
        $(".loader").hide();
    });
}
function AddTextToDb() {
    var TextData = $("#TextForm").validate().form();
    if (TextData != false) {
        $(".loader").show();
        var form = $("#TextForm");
        var url = form.attr("action");
        var formData = form.serialize();
        $.post(url, formData, function (data) {
            $("#TextForm")[0].reset();
            $("#TextForm #EditUserId").val("");
            showAllTextValue();
        });
    }

}
function showAllTextValue(tiq) {
    $.post("/Shop/TextsDisplay", function (data) {
        $("#ShopTextId").html(data);

        if (tiq != 9) {
            $(".BlockText").slideToggle();
        }
        $(".loader").hide();
    });
}


function DeleteProductWhileBooking(Id) {
    $.post("/Product/GetDataOfLanguage?Text='Do You want to Delete Product?'&id=6", function (data) {
        var confirmDelete = confirm(data)
        if (confirmDelete) {
            $(".loader").show();
            $.post("/Home/DeleteProductFromBook?Id=" + Id, function () {
                getAllBookedDataFromDb();
            });
        }
    });
}
function getAllBookedDataFromDb() {
    $.post("/Home/DisplayBookedProduct", function (data) {
        $("#MainLastBooking").html(data);
        if ($("#TotalDuration").val() == "0" || $("#TotalDuration").val() == 0) {
            GoBackToProduct();
        }
        else {
            $(".loader").hide();
        }

    });
}
function GoBackToProduct() {
    window.location.href = "/Home/ChooseYourProduct";
}
function DeleteCalProductWhileBooking(Id) {
    $.post("/Product/GetDataOfLanguage?Text='Do You want to Delete Product?'&id=6", function (data) {
        var confirmDelete = confirm(data)
        if (confirmDelete) {
            $(".loader").show();
            $.post("/Reservation/DeleteCalProduct?Id=" + Id, function () {
                window.location.href = "/Reservation/Reservation#calendarmonth";
            });
        }
    });
}

function BackToList(value) {
    $.post("/Customer/DisplayCustomerHistory", function (data) {
        $("#HistoryInfoCustomer").html(data);
    });
    $("#ClientNewForm")[0].reset();
    $("#status").val("");
    $("#NWTitle").val("");
    $("#NWEditUserId").val("");
    $.post("/Product/GetCommonLangauge?PageName=Customer_New_Customer&orderid=15", function (data) {
        $("#TotalRevenue").html(data + " :");
    });
    $.post("/Product/GetCommonLangauge?PageName=Customer_New_Customer&orderid=16", function (data) {
        $("#custSince").html(data + " :");
    });
    $("#blackListValue").attr("checked", false);
    $.post("/Product/GetCommonLangauge?PageName=Customer_Tab&orderid=3", function (data) {
        $("#NewCustomerLI a").html(data);
    });
    $("#Cancelnewcustomer").show();
    $("#CancelnewcustomerBackToBlackList").hide();
    $("#CancelnewcustomerBackToViewList").hide();
    if (value == 'A') {
        if ($(window).width() < 768)
            $("#listview-collapse").parent().find("a:first").click();
        else
            $("#ListViewLI a").click();
        //$("#newcustomer,#newcustomer-collapse").removeClass("in active");
        //$("#NewCustomerLI,#NewCustomerLI-collapse").removeClass("active");
        //$("#listview,#listview-collapse").addClass("in active");
        //$("#ListViewLI,#ListViewLI-collapse").addClass("active");
    }
    else if (value = 'L') {
        if ($(window).width() < 768)
            $("#blacklist-collapse").parent().find("a:first").click();
        else
            $("#BlackListLI a").click();
        //$("#newcustomer,#newcustomer-collapse").removeClass("in active");
        //$("#NewCustomerLI,#NewCustomerLI-collapse").removeClass("active");
        //$("#blacklist,#blacklist-collapse").addClass("in active");
        //$("#BlackListLI,#BlackListLI-collapse").addClass("active");
    }
}
function TransferToOther() {
    $(".loader").show();
    var ab = $('#ProductUserId').val();
    var UserId = $("#CopyEmployee").val();
    $.post("/Employee/TransferEmployeeProduct?EmpUserId=" + ab + "&CopyFrom=" + UserId, function () {
        $("#AllProduct").hide();
        $("#DisplayCopy").hide();
        EmployeeProductDisplay(ab);
    });
}
function GoToEmployeeMaster() {
    window.location.href = "/Employee/EmployeeMaster";
}
function GoToPendingAccepted(EmpSchId) {
    $.post("/Product/GetDataOfLanguage?Text='Do You want To Return the Request?'&id=20", function (data) {
        var GoToPending = confirm(data);
        if (GoToPending) {
            $(".loader").show();
            var Status = 0;
            $.post("/Reservation/GoBackToPending?EmpSchDetailsId=" + EmpSchId, function () {
                GoToSelected(Status);
                MoveTo();
            });
        }
    });

}
function GoToPendingOpenListView(EmpSchId) {
    $.post("/Product/GetDataOfLanguage?Text='Do You want To Return the Request?'&id=20", function (data) {
        var GoToPending = confirm(data);
        if (GoToPending) {
            $.post("/Reservation/ReservationValidationCheck?id=" + EmpSchId, function (Result) {
                if (Result == "AlreadyRegistered") {
                    $.post("/Product/GetDataOfLanguage?Text=" + Text + "&id=48", function (data1) {
                        $("#WelcomeAlert").html("<h1 class=\"text-center margin-bottom30\"><span>CLICK-AND-GO</span></h1><p class=\"title15 text-center margin-bottom30\">" + data1 + "</p>");
                        $('#welcome').modal("show");
                    });
                }
                else {
                    $(".loader").show();
                    var Status = "Pending";
                    $.post("/Reservation/BackToPending?EmpSchDetailsId=" + EmpSchId, function () {
                        //$.post("/Reservation/PendingForListViewDetail?Status=" + Status + "", function (PendingData) {
                        //    $("#ListOpenViewBody").html(PendingData);
                        //   // $(".DropListViewDet").html(Status + '<span class="caret"></span>');
                        //    $(".loader").hide();
                        //});
                        AllDataPls();
                    });
                }
            });
        }
    });
}

function GoToPendingDecline(EmpSchId) {
    var Text = "";
    $.post("/Product/GetDataOfLanguage?Text='Do You want To Return the Request?'&id=20", function (data) {
        var GoToPending = confirm(data);
        if (GoToPending) {
            $.post("/Reservation/ReservationValidationCheck?id=" + EmpSchId, function (Result) {
                if (Result == "AlreadyRegistered" || Result == "Leaves") {
                    if (Result == "AlreadyRegistered") {
                        $.post("/Product/GetDataOfLanguage?Text=" + Text + "&id=48", function (data1) {
                            $("#WelcomeAlert").html("<h1 class=\"text-center margin-bottom30\"><span>CLICK-AND-GO</span></h1><p class=\"title15 text-center margin-bottom30\">" + data1 + "</p>");
                            $('#welcome').modal("show");
                        });
                    }
                    else {
                        $.post("/Product/GetDataOfLanguage?Text=" + Text + "&id=52", function (data1) {
                            $("#WelcomeAlert").html("<h1 class=\"text-center margin-bottom30\"><span>CLICK-AND-GO</span></h1><p class=\"title15 text-center margin-bottom30\">" + data1 + "</p>");
                            $('#welcome').modal("show");
                        });
                    }
                }
                else {
                    $(".loader").show();
                    var Status = 1;
                    $.post("/Reservation/GoBackToPending?EmpSchDetailsId=" + EmpSchId, function () {
                        GoToSelected(Status);
                        MoveTo();
                    });
                }
            });
        }

    });
}
function GoToSelected(Back) {
    var status = "";
    if (Back == 0) { status = "Accepted"; }
    if (Back == 1) { status = "Declined"; }
    $.post("/Reservation/PendingForApprovalDetail?status=" + status + "", function (data) {
        $("#PendingDataDefault").html(data);
        $(".DropPendingDef").html(status + '<span class="caret"></span>');
    });
}
function DeleteEmployeeData(UserId) {
    $.post("/Product/GetDataOfLanguage?Text='Do You Want to Delete Employee?'&id=25", function (data) {
        var Exist = confirm(data)
        if (Exist) {
            $.post("/Employee/DeleteEmployee?UserId=" + UserId, function () {
                GoToEmployeeMaster();
            });
        }
    });

}
function GoToEmployeeMaster() {
    window.location.href = "/Employee/EmployeeMaster";
}
function moveConfirmBooking(ClientUserId) {
    //loader 
    $(".loader").show();
    //Get EmpId
    var EmpUserId = $("#UserIdOrder").val();
    //condition when there is no Emp
    if (EmpUserId == "" || EmpUserId == null || EmpUserId == undefined) {
        $.post("/Product/GetDataOfLanguage?Text='Please Select Employee'&id=8", function (data) {
            $(".loader").hide();
            $("#MsgAlert").html("<h2 class=\"modal-h4 text-center\">" + data + "</h2>");
            $('#myModal').modal("show");
        });
    }
    else {
        //Date Selected
        var Date = $(".k-days .k-selected").html() + " " + moment($.trim($(".k-caption").html().split(',')[0]), 'MMM').lang('en').format("MMMM") + ", " + $.trim($(".k-caption").html().split(',')[1]);
        if ($(".k-days .k-selected").html() == "" || $(".k-days .k-selected").html() == undefined || $(".k-days .k-selected").html() == null || $(".k-caption").html() == "" || $(".k-caption").html() == undefined || $(".k-caption").html() == null) {
            //Language conversion
            $.post("/Product/GetDataOfLanguage?Text='Please Select Date'&id=9", function (data) {
                //Loader Hide
                $(".loader").hide();
                //pop up
                $("#MsgAlert").html("<h2 class=\"modal-h4 text-center\">" + data + "</h2>");
                $('#myModal').modal("show");
            });

        }
        else {

            var SlotTime = $("#TimeSlotDataForm .active").html();
            if (SlotTime == "" || SlotTime == null || SlotTime == undefined) {
                $.post("/Product/GetDataOfLanguage?Text='Please Choose Time for Booking'&id=10", function (data) {
                    $(".loader").hide();
                    $("#MsgAlert").html("<h2 class=\"modal-h4 text-center\">" + data + "</h2>");
                    $('#myModal').modal("show");
                });

            }
            else {
                $.post("/Home/GoToNextBookedStatus?DateBooked=" + Date + "&EmpUserId=" + EmpUserId + "&ClientUserId=" + ClientUserId + "&slotTime=" + SlotTime, function (Result) {
                    if (Result == "Error") {
                        window.location.href = "/Home/ChooseYourProduct";

                    }
                    else {
                        window.location.href = "/Home/ConfirmBooking";
                    }

                });
            }

        }
    }
}
function moveConfirmBookingShop(ClientUserId, Status, Url) {
    $(".loader").show();
    var EmpUserId = $("#UserIdOrder").val();
    if (EmpUserId == "" || EmpUserId == null || EmpUserId == undefined) {
        $.post("/Product/GetDataOfLanguage?Text='Please Select Employee'&id=8", function (data) {
            $(".loader").hide();
            $("#MsgAlert").html("<h2 class=\"modal-h4 text-center\">" + data + "</h2>");
            $('#myModal').modal("show");

        });
    }
    else {

        //var Date = $(".k-days .k-selected").html() + " " + $(".k-caption").prop("id");
        var Date = $(".k-days .k-selected").html() + " " + moment($.trim($(".k-caption").html().split(',')[0]), 'MMM').lang('en').format("MMMM") + ", " + $.trim($(".k-caption").html().split(',')[1]);
        if ($(".k-days .k-selected").html() == "" || $(".k-days .k-selected").html() == undefined || $(".k-days .k-selected").html() == null || $(".k-caption").html() == "" || $(".k-caption").html() == undefined || $(".k-caption").html() == null) {
            $.post("/Product/GetDataOfLanguage?Text='Please Select Date'&id=9", function (data) {
                $(".loader").hide();
                $("#MsgAlert").html("<h2 class=\"modal-h4 text-center\">" + data + "</h2>");
                $('#myModal').modal("show");
            });

        }
        else {
            var SlotTime = $("#TimeSlotDataForm .active").html();
            if (SlotTime == "" || SlotTime == null || SlotTime == undefined) {
                $.post("/Product/GetDataOfLanguage?Text='Please Choose Time for Booking'&id=10", function (data) {
                    $(".loader").hide();
                    $("#MsgAlert").html("<h2 class=\"modal-h4 text-center\">" + data + "</h2>");
                    $('#myModal').modal("show");
                });
            }
            else {
                $.post("/Home/GoToNextBookedStatus?DateBooked=" + Date + "&EmpUserId=" + EmpUserId + "&ClientUserId=" + ClientUserId + "&slotTime=" + SlotTime, function () {
                    window.location.href = "/BookOrder/ConfirmBooking?Id=" + ClientUserId + "&Status=" + Status + "&Url=" + Url;
                });
            }
        }
    }
}
function UpdateReservation(ReservationId, CatgTypeId, Status) {
    $(".loader").show();
    var Msg = "";
    if ($("#UserIdOrder").val() == "") {
        Msg = "Error";
        $.post("/Product/GetDataOfLanguage?Text='Please Select Employee'&id=8", function (data) {
            $(".loader").hide();
            $("#MsgAlert").html("<h2 class=\"modal-h4 text-center\">" + data + "</h2>");
            $('#myModal').modal("show");
        });
    }
    if ($("#choosedTime").val() == "") {
        Msg = "Error";
        $.post("/Product/GetDataOfLanguage?Text='Please Choose Time for Booking'&id=10", function (data) {
            $(".loader").hide();
            $("#MsgAlert").html("<h2 class=\"modal-h4 text-center\">" + data + "</h2>");
            $('#myModal').modal("show");
        });
    }
    if ($("#ChoosedDate").val() == "") {
        Msg = "Error";
        $.post("/Product/GetDataOfLanguage?Text='Please Select Date'&id=9", function (data) {
            $(".loader").hide();
            $("#MsgAlert").html("<h2 class=\"modal-h4 text-center\">" + data + "</h2>");
            $('#myModal').modal("show");
        });
    }
    if (Msg != "Error") {
        var form = $("#UpdateReservationForm");
        var url = form.attr("action");
        var formData = form.serialize();
        $.post(url, formData, function (data) {
            $("#UpdateReservationForm")[0].reset();
            if ($("#ReservationStatus").val() == "CalUpdateReservation") {
                window.location.href = "/Reservation/Reservation#calendarmonth";
            }
            else {
                window.location.href = "/Reservation/Reservation#openlistview";
            }

        });
    }
}
function DeleteProductWhileBookingShop(Id, UserId, Status) {
    $.post("/Product/GetDataOfLanguage?Text='Do You want to Delete Product?'&id=6", function (data) {
        var confirmDelete = confirm(data);
        if (confirmDelete) {
            $(".loader").show();
            $.post("/Home/DeleteProductFromBook?Id=" + Id, function () {
                getAllBookedDataFromDbShop(UserId, Status);
            });
        }
    });

}

function gotoThanks(UserId, Status, Url) {
    window.location.href = "/BookOrder/ThanksMessage?Id=" + UserId + "&Status=" + Status + "&Url=" + Url;
}
function BookMyOrderShop(UserId, Status, Url) {
    $(".loader").show();
    var Comments = $("#Comments").val();
    $.post("/BookOrder/BookMyOrder?Id=" + UserId + "&Comments=" + Comments + "&Status=" + Status + "&Url=" + Url, function (Result) {
        if (Result == "DCerror")
            window.location.href = "/Customer/Customer";
        else if (Result == "DOerror")
            window.location.href = "/Reservation/Reservation#" + Url;
        else
            gotoThanks(UserId, Status, Url);
    });
}
function getAllBookedDataFromDbShop(Id, Status) {
    $.post("/BookOrder/DisplayBookedProduct?Id=" + Id + "&Status=" + Status, function (data) {
        $("#MainLastBooking").html(data);
        if ($("#TotalDuration").val() == "0" || $("#TotalDuration").val() == 0) {
            GoBackToProductShop(Id, Status);
        }
        else {
            $(".loader").hide();
        }
    });
}
function GoBackToProductShop(Id, Status) {
    window.location.href = "/BookOrder/OrderList?id=" + Id + "&Status=" + Status;
}
//ankit code
function ChangeNewPassword() {
    var TextData = $("#ChangePasswordForm").validate().form();
    if (TextData != false) {
        $(".loader").show();
        var form = $("#ChangePasswordForm");
        var url = form.attr("action");
        var formData = form.serialize();
        $.post(url, formData, function (data) {
            $('#welcome').modal("show");
        });
    }
}
function SaveNewPassword() {
    var TextData = $("#ChangePasswordForm").validate().form();
    if (TextData != false) {
        $(".loader").show();
        var form = $("#ChangePasswordForm");
        var url = form.attr("action");
        var formData = form.serialize();
        $.post(url, formData, function (data) {
            if (data == "Success") {
                $.post("/Product/GetDataOfLanguage?Text='Greeting From'&id=22", function (data1) {
                    $.post("/Product/GetDataOfLanguage?Text='Your Password Changed Successfully'&id=23", function (data2) {
                        $(".loader").hide();
                        //Here Code Will Change
                        $("#WelcomeAlert").html("<h1 class=\" text-center margin-bottom30\">" + data1 + "<span>CLICK-AND-GO!</span></h1><h2 class=\"modal-h4 text-center\">" + data2 + "</h2><div class=\"text-right\"><a class=\"btn btn-green\" href=\"/Shop/Shop\">Ok</a></div>");
                        $('#welcome').modal("show");
                    });

                });
            }
            else {
                $.post("/Product/GetDataOfLanguage?Text='Sorry Your Current Password Does Not Match/Error in Changing'&id=24", function (data) {
                    $(".loader").hide();
                    $("#MsgAlert").html("<h2 class=\"modal-h4 text-center\">" + data + "</h2>");
                    $('#myModal').modal("show");
                });
            }
        });
    }
}
function SkipNewCustomerValidation() {
    $(".loader").show();
    $.post("/Product/GetDataOfLanguage?Text='Do you want to skip validation ?'&id=42", function (data) {
        $(".loader").hide();
        var SkipValidation = confirm(data);
        if (SkipValidation) {
            $(".loader").show();
            var form = $("#ClientNewForm");
            var url = form.attr("action");
            var formData = form.serialize();
            $.post(url, formData, function (data) {
                $("#ClientNewForm")[0].reset();
                window.location.href = "/Customer/Customer";
            });
        }
        else {
            var SkipValidationCheck = $("#ClientNewForm").validate().form();
            if (SkipValidationCheck != false) {
                var form = $("#ClientNewForm");
                var url = form.attr("action");
                var formData = form.serialize();
                $.post(url, formData, function (data) {
                    $("#ClientNewForm")[0].reset();
                    window.location.href = "/Customer/Customer";
                });
            }
        }
    });
}
function EditLanguage(Id) {
    $(".loader").show();
    $.ajax({
        url: "/Shop/EditDisplayLangauge?id=" + Id,
        cache: false,
        dataType: "json",
        success: function (Languagedata) {
            $("#DivLanguageEdit").slideToggle();
            $("#LanguageId").val(Languagedata.LangDetailId);
            $("#LanguageText").val(Languagedata.LangValue);
            $(".loader").hide();
        },
        error: function (request, status, error) { }
    });
}
function EditLanguageAdd() {
    $(".loader").show();
    var form = $("#LanguageForm");
    var url = form.attr("action");
    var formData = form.serialize();
    $.post(url, formData, function (data) {
        $.post("/Shop/Language_LabelDetail?PageName=" + data, function (data) {
            $("#DisplayContent").html(data);
            $("#LanguageForm")[0].reset();
            $("#LanguageId").val("");
            $("#DivLanguageEdit").slideToggle();
            $(".loader").hide();
        })
    });
}
function GetConversion(Text, Id) {
    $(".loader").show();
    $.post("/Product/GetDataOfLanguage?Text=" + Text + "&id=" + Id, function (data) {
        $(".loader").hide();
    });
}
function DayChangeFunction(UserId, status) {
    var date = $.trim($("#Sub_Heading_" + UserId).html());
    $(".loader").show();
    $.post("/Reservation/DayChange?UserId=" + UserId + "&DateandDay=" + date + "&status=" + status, function (data) {
        $("#Body_" + UserId).html(data);
        $.ajax({
            url: "/Reservation/GetAllDayDisplay?date=" + date + "&status=" + status + "&from=DayIn",
            cache: false,
            dataType: "json",
            success: function (DataDisplay) {
                if (DataDisplay.DisplayHeading != "")
                    $("#Sub_Heading_" + UserId).html(DataDisplay.DisplayHeading);
                if (DataDisplay.DisplaySubheading != "")
                    $("#Main_Heading_" + UserId).html(DataDisplay.DisplaySubheading);
                $(".loader").hide();
            },
            error: function (request, status, error) { }
        });
    });
}
function AddGroupText() {
    var grouptext = $("#GroupTextForm").validate().form();
    if (grouptext != false) {
        $(".loader").show();
        var form = $("#GroupTextForm");
        var url = form.attr("action");
        var formData = form.serialize();
        $.post(url, formData, function (data) {
            $("#GroupTextForm")[0].reset()
            $(".BlockGroupText").slideToggle();
            ShowProductDetail();
            $(".loader").hide();
        });
    }
}
function AddGroupProductToDb() {
    var NullTempValue = "";
    if ($("#EditCatgTypeId").val() != "") {
        NullTempValue = "yes";
    }
    $('.loader').show();
    var form = $("#AddProductData");
    var url = form.attr("action");
    var formData = form.serialize();
    $.post(url, formData, function (data) {
        $("#AddProductData")[0].reset();
        $(".add-vacation-block").slideToggle();
        $('#welcome').modal("hide");
        if (NullTempValue == "yes") {
            $("#EditCatgTypeId").val("");
        }
        if (data == "yes") {
            ShowProductDetailAdd();
        }
        else {
            ShowProductDetail();
        }

    });

}
function AddProductToDb(GenderLangText) {
    if ($("#SelectedInsurance").val() == "") {
        $("#InsuranceDropDown").val("");
    }
    if (($('#Female').is(':checked') || $('#male').is(':checked'))) {
        $("#Gendervalidation").html(' ');
        var addprodvalidation = $("#AddProductData").validate().form();
        if (addprodvalidation != false) {
            AddGroupProductToDb();
        }
    }
    else {
        $("#Gendervalidation").html(GenderLangText)
        $("#AddProductData").validate().form();
    }

}
function EditGroupName(CatgTypeId) {
    window.scrollTo(0, 0);
    $(".loader").show();
    $(".BlockGroupText").slideDown();
    $.ajax({
        url: "/Product/EditGroupNameDetail?Catgtypeid=" + CatgTypeId,
        cache: false,
        dataType: "json",
        success: function (GroupName) {
            $("#GroupName").val(GroupName.CatgDesc);
            $("#EditGroupNameCatgTypeId").val(GroupName.Catgtypeid);
            $(".loader").hide();
        },
        error: function (request, status, error) { }
    });
}
function DeleteGroupName(CatgTypeId) {
    $(".loader").show();
    $.post("/Product/GetDataOfLanguage?Text='Do You want to delete Group?'&id=54", function (data) {
        $(".loader").hide();
        var confirmDeleteGroup = confirm(data);
        if (confirmDeleteGroup) {
            $(".loader").show();
            $.post("/Product/DeleteGroupNameDetail?Catgtypeid=" + CatgTypeId, function () {
                $(".loader").hide();
                ShowProductDetail();
            });
        }
    });
}
function EditCalReservation(ReservationId, Status) {
    $(".loader").show();
    $.post("/Product/GetDataOfLanguage?Text='Do you want to Edit Booking ?'&id=51", function (data) {
        $(".loader").hide();
        var ReservationCalBookedDetails = confirm(data);
        if (ReservationCalBookedDetails) {
            $(".loader").show();
            var Calview = $("#calendarmonth .view-cal .active").prop("id");
            var view = "";
            if (Calview == "DayBTN") { view = "Day"; }
            if (Calview == "WeekBTN") {
                view = "Week";
            }
            if (Calview == "MONTHBTN") {
                view = "Month";
            }
            var AllView = $("#EmployeesDaybtn .activebutton").prop("id");
            var AllviewStatus = "";
            if (AllView == "All" && view == "Day") {
                AllviewStatus = "All";
                window.location.href = "/Reservation/UpdateReservedData?Status=" + Status + "&ReservationId=" + ReservationId + "&Viewstatus=" + view + "&AllView=" + AllviewStatus;
            }
            else {
                window.location.href = "/Reservation/UpdateReservedData?Status=" + Status + "&ReservationId=" + ReservationId + "&Viewstatus=" + view;
            }

        }
        else {
            window.location.href = "/Reservation/Reservation#calendarmonth";
        }
    });
}
function AddVarious() {
    if ($("#NumberEntered").val() == "" || $("#AdvanceBooking").val() == "" || $("#ColorSelection").val() == "" || $("#reminderInput").val() == "") {
        if ($("#NumberEntered").val() == "")
            $("#NumberError").html("please add Reminder Alert!");
        if ($("#AdvanceBooking").val() == "")
            $("#AdvanceError").html("please add Advance booking restriction!");
        if ($("#ColorSelection").val() == "")
            $("#ColorError").html("please select Colour!");
        if ($("#reminderInput").val() == "")
            $("#ReminderError").html("please select Reminder!");
    }
    else {
        var formData = new FormData($("#VariousForm")[0]);
        $(".loader").show();
        $.ajax({
            url: $("#VariousForm").attr("action"),
            type: 'POST',
            data: formData,
            async: true,
            success: function () {
                if (window.location.toString().indexOf('#') != -1) {
                    var ab = window.location.hash.substr(1);
                    if (ab == "Various") { location.reload(); }
                    else {
                        window.location.hash = 'Various';
                        window.location.reload();
                    }
                }
                else {
                    window.location.hash = '#Various';
                    window.location.reload();
                }
                $(".loader").hide();
            },
            cache: false,
            contentType: false,
            processData: false
        });
    }
}
function AddCatgdet(CatgId, CatgNames, CatgDetails, DeleteLangText) {
    $.ajax({
        url: "/Doctor/AddCategories",
        cache: false,
        data: { catgName: CatgNames, catgId: CatgId },
        async: true,
        dataType: "json",
        success: function (PatientComplaints) {
            var Length = $("#" + CatgDetails + " li").length;
            var DisplayText = "";
            var FindContent = $("#" + CatgDetails);
            if (CatgDetails == "MedicineList" || CatgDetails == "TakeList" || CatgDetails == "WhenDispList") {
                DisplayText = "<li><a id='" + PatientComplaints.CatgTypeId + "'>" + PatientComplaints.CatgDesc + "</a></li>";
                //$("#MediValMessage").text("Saved").fadeIn().delay(2000).fadeOut();
            }
            else if (CatgDetails == "followFind") {
                DisplayText = "<li><a id=" + PatientComplaints.CatgTypeId + "><label>" + PatientComplaints.CatgDesc + "</label><span class='doctor-delete'><i class='fa fa-trash' data-toggle='popover' data-trigger='hover' data-placement='top' data-content='" + DeleteLangText + "' data-original-title='' title=''></i></span></a></li>";
            }
            else
                DisplayText = "<li><a class='doctor-checkbox'><input name='" + CatgDetails + "_" + PatientComplaints.CatgTypeId + "' id='" + PatientComplaints.CatgTypeId + "' type='checkbox' value='" + PatientComplaints.CatgTypeId + "'><label for='" + PatientComplaints.CatgTypeId + "' class=''>" + PatientComplaints.CatgDesc + " </label><span class='doctor-delete'><i class='fa fa-trash' data-toggle='popover' data-trigger='hover' data-placement='top' data-content='" + DeleteLangText + "' data-original-title='' title=''></i></span></a></li>";

            FindContent.scrollTop(0);
            if (Length == 0)
                FindContent.html(DisplayText)
            else
                FindContent.find("li").first().before(DisplayText);
            if (CatgDetails == "MedicineList" || CatgDetails == "TakeList" || CatgDetails == "WhenDispList") {
                FindContent = $("#" + CatgDetails + "_Find");
                DisplayText = "<li><a id=" + PatientComplaints.CatgTypeId + " class='MedicineDetele'><label>" + PatientComplaints.CatgDesc + "</label><span class='doctor-delete'><i class='fa fa-trash' data-toggle='popover' data-trigger='hover' data-placement='top' data-content='" + DeleteLangText + "' data-original-title='' title=''></i></span></a></li>";
                if (PatientComplaints.CatgTypeId > 0) {
                    if (Length == 0)
                        FindContent.html(DisplayText)
                    else
                        FindContent.find("li").first().before(DisplayText);
                }

            }
            if (CatgDetails == "DiagnosisFind") {
                FindContent = $("#PreDiagnosisFind");
                DisplayText = "<li><a class='doctor-checkbox'><input name='Prescription_" + PatientComplaints.CatgTypeId + "'  id='PreDiagnosis_" + PatientComplaints.CatgTypeId + "' type='checkbox' value='" + PatientComplaints.CatgTypeId + "'><label for='PreDiagnosis_" + PatientComplaints.CatgTypeId + "' class=''>" + PatientComplaints.CatgDesc + " </label><span class='doctor-delete'><i class='fa fa-trash' data-toggle='popover' data-trigger='hover' data-placement='top' data-content='" + DeleteLangText + "' data-original-title='' title=''></i></span></a></li>";
                var Length = $("#PreDiagnosisFind li").length;
                if (Length == 0)
                    $("#PreDiagnosisFind").html(DisplayText)
                else
                    $("#PreDiagnosisFind").find("li").first().before(DisplayText);
            }
            $('[data-toggle="popover"]').popover();
        },
        error: function (request, status, error) { }
    });
}
function AddMedicamentsDetails(FillAllDataMsg, DeleteLangText, AlertDeleteMasg, SavedMsg, AlreadyExistMsg) {
    var Mediciene = $("#SelectedMedicine").val();
    var NoOfTime = $("#SelectedNumOfTimetake").val();
    var WhenToTake = $("#SelectedWhenToTake").val();
    if (Mediciene != "" && NoOfTime != "" && WhenToTake != "") {
        var MedicineValue = $("#MedicineList #" + Mediciene).html();
        var NoOfTimeValue = $("#TakesMediList #" + NoOfTime).html();
        var WhenValue = $("#WhenDispList #" + WhenToTake).html();
        var MedicamentFreeText = $("#textAreaMedicaments").val();
        $("#textAreaMedicaments").val("");
        var Session = "Medicments";
        var Countsession = "CountSpecial";
        AddDoctorPreDetails(Mediciene, NoOfTime, WhenToTake, MedicineValue, NoOfTimeValue, WhenValue, "MedicationDisplay", Session, Countsession, MedicamentFreeText, DeleteLangText, AlertDeleteMasg, SavedMsg, AlreadyExistMsg);
        $(".SelectedMedicine").val("");
        $(".SelectedNumOfTimetake").val("");
        $(".SelectedWhenToTake").val("");
        $("#SelectedMedicine").val("");
        $("#SelectedNumOfTimetake").val("");
        $("#SelectedWhenToTake").val("");
    }
    else {
        $("#statusMed").text(FillAllDataMsg).fadeIn().delay(2000).fadeOut();
    }
}

function AddDoctorAdvice(FillAllDataMsg, DeleteLangText, AlertDeleteMasg, SavedMsg, AlreadyExistMsg) {
    var Mediciene = $("#SelectedMedicinePre").val();
    var NoOfTime = $("#SelectedNumOfTimetakePre").val();
    var WhenToTake = $("#SelectedWhenToTakePre").val();
    var MedicineValue = $("#MedicineListPre #" + Mediciene).html();
    var NoOfTimeValue = $("#TakesMediListPre #" + NoOfTime).html();
    var WhenValue = $("#WhenDispListPre #" + WhenToTake).html();
    var Session = "Advice";
    var Countsession = "CountAdvice";
    var AdviceFreeText = $("#textAdvice").val();
    $("#textAdvice").val("");
    AddDoctorPreDetails(Mediciene, NoOfTime, WhenToTake, MedicineValue, NoOfTimeValue, WhenValue, "AdviceDisplay", Session, Countsession, AdviceFreeText, DeleteLangText, AlertDeleteMasg, SavedMsg, AlreadyExistMsg);

}
function AddDoctorPreDetails(Mediciene, NoOfTime, WhenToTake, MedicineValue, NoOfTimeValue, WhenValue, displayid, Session, CountSession, FreeText, DeleteLangText, AlertDeleteMasg, SavedMsg, AlreadyExistMsg) {
    var internal = {
        Medicine: Mediciene,
        HowManyTimes: NoOfTime,
        When: WhenToTake,
        AddPreDetails: Session,
        CountSpecial: CountSession,
        FreeText: FreeText
    };
    $.ajax({
        url: "/Doctor/TempMedicamentsdata",
        contentType: "application/json",
        data: JSON.stringify(internal),
        datatype: "json",
        type: 'POST',
        success: function (Id) {
            if (Id != 0) {
                //var DisplayHtml = "<li><div class='col-lg-21 col-md-21 col-sm-21 col-xs-21'>" + MedicineValue + ",&nbsp&nbsp" + NoOfTimeValue + ",&nbsp&nbsp" + WhenValue + "&nbsp&nbsp" + FreeText + "</div><div class='col-lg-3 col-md-3 col-sm-3 col-xs-3'><a id='" + Id + "'><i class='fa fa-trash spa-icon1 sure-span' data-toggle='popover' data-trigger='hover' data-placement='top' data-content='Delete' data-original-title='' title=''></i><span class='pull-left display-none color-blue'>Sure ?</span></a></div></li>";
                var DisplayHtml = " <tr><td class='width20per'>" + MedicineValue + "</td><td class='width20per'>" + NoOfTimeValue + "</td><td class='width20per'>" + WhenValue + "</td><td class='width30per'>" + FreeText + "</td><td class='width10per'><a id='" + Id + "'><i class='fa fa-trash spa-icon1 sure-span' data-toggle='popover' data-trigger='hover' data-content='" + DeleteLangText + "' data-placement='top' data-original-title='' title=''></i><span class='pull-left display-none color-blue'>" + AlertDeleteMasg + "</span></a></td></tr>";
                if ($("#" + displayid + " tr").length == 0)
                    $("#" + displayid).html(DisplayHtml);
                else
                    $("#" + displayid + " tr").first().before(DisplayHtml);

                if ($("#AdviceDisplay" + " tr").length == 0)
                    $("#AdviceDisplay").html(DisplayHtml);
                else
                    $("#AdviceDisplay" + " tr").first().before(DisplayHtml);
                $("#statusMed").text(SavedMsg).fadeIn().delay(2000).fadeOut();
                $('[data-toggle="popover"]').popover();
            }
            else
                $("#statusMed").text(AlreadyExistMsg).fadeIn().delay(2000).fadeOut();
        }
    });
}
function AddSelectedCatg(IdList, DeleteLangText, AddTextLang) {
    $(IdList).click(function () {
        var CatgId = 0;
        var CatgDetails = "";
        var Patient = "";
        var PatientGet = "";
        var SelectedId = $.trim($(this).prop("id"))
        CatgId = $(this).attr("Name");
        CatgDetails = $(this).parents(".BorderName").find(".CatgUl").prop("id");
        if (SelectedId == "PatientAdd") {
            CatgId = 131;
            CatgDetails = "patientFind";
        }
        if (SelectedId == "DiagnosisAdd" || SelectedId == "PreDiagnosisAdd") {
            CatgId = 132;
            CatgDetails = "DiagnosisFind";
        }
        if (SelectedId == "InvestigationAdd") {
            CatgId = 135;
            CatgDetails = "InvestigationFind";
        }
        if (SelectedId == "AllergiesAdd") {
            CatgId = 136;
            CatgDetails = "AllergiesFind";
        }
        if (SelectedId == "followAdd") {
            CatgId = 139;
            CatgDetails = "followFind";
        }
        if (SelectedId == "Add_MedicineList") {
            CatgId = 130;
            CatgDetails = "MedicineList";
        }
        if (SelectedId == "Add_TakeList") {
            CatgId = 134;
            CatgDetails = "TakeList";
        }
        if (SelectedId == "Add_WhenDispList") {
            CatgId = 135;
            CatgDetails = "WhenDispList";
        }
        if (SelectedId == "AddMedication" || SelectedId == "AddAdvice") {
            if ($("#AddMedicine").prop("checked")) {
                CatgId = 130;
                CatgDetails = "MedicineListPre";
                if (SelectedId == "AddMedication")
                    CatgDetails = "MedicineList";
            }
            if ($("#AddWhenToTake").prop("checked")) {
                CatgId = 134;
                CatgDetails = "TakesMediListPre";
                if (SelectedId == "AddMedication")
                    CatgDetails = "TakeList";

            }
            if ($("#AddNoOfTimes").prop("checked")) {
                CatgId = 133;
                CatgDetails = "WhenDispListPre";
                if (SelectedId == "AddMedication")
                    CatgDetails = "WhenDispList";
            }
        }
        if (SelectedId == "AddMedication" || SelectedId == "AddAdvice") {
            PatientGet = $("#txtAddMedicine");
            Patient = PatientGet.val();
        }
        else {
            PatientGet = $(this).parents(".Main-Div").find("input[type='text']");
            Patient = PatientGet.val();

        }
        if (Patient != "") {
            var PatientComplaints = AddCatgdet(CatgId, Patient, CatgDetails, DeleteLangText);
            $("#" + CatgDetails).removeClass("display-none").parent().find(".NewAddlist").addClass("display-none");
            $("#" + CatgDetails).find("li[class='display-none']").removeClass("display-none");
        }
        else {
            if (SelectedId == "AddMedication" || SelectedId == "AddAdvice")
                $("#MediValMessage").text(AddTextLang).fadeIn().delay(2000).fadeOut();
            if (SelectedId == "PreDiagnosisAdd")
                $("#PreDiagnosisFind_Border").removeClass("border-green").addClass("border-red").delay(2000).queue(function () { $("#PreDiagnosisFind_Border").removeClass("border-red").addClass("border-green").clearQueue() });
            else
                $("#" + CatgDetails + "_Border").removeClass("border-green").addClass("border-red").delay(2000).queue(function () { $("#" + CatgDetails + "_Border").removeClass("border-red").addClass("border-green").clearQueue() });
        }
        PatientGet.val("");
        $("#AddMedicine").prop("checked", true);
    });
}
function SelectedCheckbox(IdList, DeleteLangMsg) {
    $("body").delegate(IdList, "click", function () {
        var DisplayText = "";
        var FindTable = $(this).closest('ul');
        var DisplayTable = $("#" + FindTable.prop("id") + "_Display");
        var ClickId = $(this).prop("id");
        if ($(this).prop("checked")) {
            var Length = DisplayTable.find("li").length;
            var Content = $.trim($(this).closest("a").find("label").html());
            if (FindTable.prop("id") == "InvestigationFind")
                DisplayText = "<li id='" + ClickId + "'><div class='col-lg-21 col-md-21 col-sm-21 col-xs-21'>" + Content + "</div><div class='col-lg-3 col-md-3 col-sm-3 col-xs-3'><a><i class='fa fa-trash spa-icon1'data-toggle='popover' data-trigger='hover' data-placement='top' data-content='" + DeleteLangMsg + "' data-original-title='' title=''></i></a></div></li>";
            else
                DisplayText = "<li id='" + ClickId + "'><a class=''>" + Content + "</a></li>";
            if (Length == 0)
                DisplayTable.html(DisplayText);
            else
                DisplayTable.find("li").first().before(DisplayText);
            $('[data-toggle="popover"]').popover();
            //bind Event to Display Section
            DisplayTable.find("li a").unbind().bind("click", function () {
                var FindTableDisplay = $(this).closest("ul").prop("id");
                FindTableDisplay = FindTableDisplay.replace("_Display", "");
                var Id = $(this).closest("li").prop("id");
                var WhereClick = $("#" + FindTableDisplay + " #" + Id);
                if (WhereClick.prop("checked"))
                    WhereClick.click();
            });
        }
        else {
            DisplayTable.find("[id='" + ClickId + "']").remove();
        }
    });
}
//new changes for deselect Diagnosis 
function SelectedDiagnosisCheckbox() {
    $("body").delegate("#DiagnosisFind li a input[type='checkbox']", "click", function () {
        var FindTable = $(this).closest('ul');
        var DisplayTable = $("#" + FindTable.prop("id") + "_Display");
        var ClickId = $(this).prop("id");
        if ($(this).prop("checked")) {
            var Length = DisplayTable.find("li").length;
            var Content = $.trim($(this).closest("a").find("label").html());
            var DisplayText = "<li id='" + ClickId + "'><a class=''>" + Content + "</a></li>";
            if (Length == 0)
                $("#DiagnosisFind_Display,#PreDiagnosisFind_Display").html(DisplayText);
            else
                $("#DiagnosisFind_Display li:first,#PreDiagnosisFind_Display li:first").before(DisplayText);
            $('[data-toggle="popover"]').popover();
            //bind Event to Display Section
            $("#DiagnosisFind_Display,#PreDiagnosisFind_Display").find("li a").unbind().bind("click", function () {
                var FindTableDisplay = $(this).closest("ul").prop("id");
                FindTableDisplay = FindTableDisplay.replace("_Display", "");
                var Id = $(this).closest("li").prop("id");
                var WhereClick = $("#DiagnosisFind #" + Id);
                if (WhereClick.prop("checked"))
                    WhereClick.click();
            });
        }
        else {
            $("#DiagnosisFind_Display,#PreDiagnosisFind_Display").find("[id='" + ClickId + "']").remove();
        }
    });
}
function FillDropDown(IdList) {
    $("body").delegate(IdList, "click", function () {
        var GetDefaultDetail = $(this);
        var ChangingUL = GetDefaultDetail.closest("ul");
        var IdforHidden = "";
        var GetId = "";
        var GetHtml = "";
        if (ChangingUL.hasClass("dropMedicine"))
            IdforHidden = "SelectedMedicine";
        if (ChangingUL.hasClass("dropNumOfTimetake"))
            IdforHidden = "SelectedNumOfTimetake";
        if (ChangingUL.hasClass("dropWhenToTake"))
            IdforHidden = "SelectedWhenToTake";
        if (ChangingUL.hasClass("dropMedicinePre"))
            IdforHidden = "SelectedMedicinePre";
        if (ChangingUL.hasClass("dropNumOfTimetakePre"))
            IdforHidden = "SelectedNumOfTimetakePre";
        if (ChangingUL.hasClass("dropWhenToTakePre"))
            IdforHidden = "SelectedWhenToTakePre";
        if (ChangingUL.hasClass("dropFollowup")) {
            IdforHidden = "textareaFollowUp";
            GetId = $.trim(GetDefaultDetail.text());
        }
        else {
            GetHtml = GetDefaultDetail.html();
            GetId = GetDefaultDetail.prop("id");
        }
        $("#" + IdforHidden).val(GetId);
        $("." + IdforHidden).val(GetHtml)
    });
}
function NoteFillDropDown(IdList) {
    $("body").delegate(IdList, "click", function () {
        var GetDefaultDetail = $(this);
        var Textobj = GetDefaultDetail.closest("ul").parent().find("input[type='text']");
        Textobj.val($.trim(GetDefaultDetail.text()));
    });
}
function DelDoctorPreDetails() {
    $("body").delegate("#MedicationDisplay tr a ,#AdviceDisplay tr a", "click", function () {
        var Id = $(this).prop("id");
        //Ask for Sure while you are deleting
        $(this).find("span").show();
        $(this).find("span").click(function () {
            var SessionName = "Medicments";
            var CountName = "CountSpecial";
            $("#MedicationDisplay tr a[id=" + Id + "],#AdviceDisplay tr a[id=" + Id + "]").parents("tr").remove();
            $.ajax({
                url: "/Doctor/DeleteMedication",
                method: "post",
                data: { id: Id, SessionName: SessionName, CountName: CountName },
                success: function () {
                }
            });
        });
    });
}
function DeleteSelectedItem(IdList) {
    $("body").delegate(IdList, "click", function () {
        var getClosestLI = $(this).closest("li");
        var Id = "";
        //For id of DropDownitem 
        if (getClosestLI.closest('ul').prop("id") == "followFind")
            Id = $(this).closest("a").prop("id");
        else
            Id = $(this).closest("a").find("input").prop("id");
        if (getClosestLI.find("input").prop("checked"))
            getClosestLI.find("#" + Id).click();
        getClosestLI.remove();
        $.ajax({
            url: "/Doctor/DeleteSelectedItem",
            method: "post",
            data: { id: Id.replace("PreDiagnosis_", "") },
            success: function () { }
        })
    });
}
function DeleteDropDownListItem() {
    $("body").delegate("#MedicineList_Find li a span,#TakeList_Find li a span,#WhenDispList_Find li a span", "click", function () {

        var getClosestLI = $(this).closest("li");
        var getClosestUl = $(this).closest("ul").prop("id").replace("_Find", "");
        var Id = $(this).closest("a").prop("id");
        getClosestLI.remove();
        $("#" + getClosestUl + " li a[id=" + Id + "]").parents("li").remove();
        $.post("/Doctor/DeleteMedicationList?Catgtypeid=" + Id, function (Id) {
            if (Id != "")
                $("#MedicationDisplay li a[id=" + Id + "],#AdviceDisplay li a[id=" + Id + "]").parents("li").remove();
        });
        $.ajax({
            url: "/Doctor/DeleteSelectedItem",
            method: "post",
            data: { id: Id },
            success: function () { }
        })
    });
}
function EditDoctorNotes(ReservationId, diff) {
    $.ajax({
        url: "/Doctor/EditDoctorNotes",
        type: 'POST',
        data: { ReservationId: ReservationId, Diff: diff },
        dataType: "json",
        async: true,
        success: function (Details) {
            $("#textareaPatient").val(Details.FreePatient);
            $("#textareaDiagnosis").val(Details.DiagFree);
            $("#textareaMedicamentDetails").val(Details.FreeMedication);
            $("#Tempreature").val(Details.Temp);
            $("#Pulse").val(Details.pulse);
            $("#BloodPressure").val(Details.BP);
            $("#RespiratoryRate").val(Details.RepositoryRate);
            $("#textareaAllergies").val(Details.FreeAllergies);
            $("#textareaPreMedicine").val(Details.FreepreDiag);
            $("#textareaAdvise").val(Details.FreeAdvise);
            $("#textareaFollowUp").val(Details.FreeFollowUp);
            $("#" + Details.CatgIDPatient).click();
            $(Details.PreDiagnosisId).click();
        },
        cache: false,
    });
    return false;
}
//function SavePrescription(FormId, btnStatus, ReservationId, EmailId) {
//    var Diff = $("#Diff").val();
//    var PrintAction = "PrescriptionCard";
//    if (Diff == 3)
//        PrintAction = "PrescriptionCard";
//    $(".loader").show();
//    var form = $("#" + FormId);
//    var url = form.attr("action");
//    var formData = form.serialize();
//    $.post(url, formData, function (data) {
//        if (data != null) {
//            if (btnStatus == "SaveMailAndPrint") {
//                $.post("/Doctor/MailPrescriptionCard?Reservationid=" + ReservationId + "&EmailID=" + EmailId + "&Diff=" + Diff, function () {
//                    //window.open("/Doctor/PrescriptionCard?Reservationid=" + ReservationId);
//                    var win = window.open();
//                    win.location = "/Doctor/PrescriptionCard?Reservationid=" + ReservationId;
//                    AppClose(ReservationId);
//                });
//            }
//            else {
//                var win = window.open();
//                win.location = "/Doctor/PrescriptionCard?Reservationid=" + ReservationId;
//                //window.open("/Doctor/PrescriptionCard?Reservationid=" + ReservationId);
//                AppClose(ReservationId);
//            }
//        }
//    });
//}
function SavePrescription(FormId, btnStatus, ReservationId, EmailId, Url) {
    var Diff = $("#Diff").val();
    var PrintAction = "PrescriptionCard";
    if (Diff == 3)
        PrintAction = "PrescriptionCard";
    $(".loader").show();
    var form = $("#" + FormId);
    var url = form.attr("action");
    var formData = form.serialize();
    $.post(url, formData, function (data) {
        if (data != null) {
            if (btnStatus == "SaveMailAndPrint") {
                $.post("/Doctor/MailPrescriptionCard?Reservationid=" + ReservationId + "&EmailID=" + EmailId + "&Diff=" + Diff, function () {
                    //window.open("/Doctor/PrescriptionCard?Reservationid=" + ReservationId);
                    //var win = window.open();
                    window.location.href = "/Doctor/PrescriptionCard?Reservationid=" + ReservationId + "&Url=" + Url;
                    //win.location = "/Doctor/PrescriptionCard?Reservationid=" + ReservationId;
                    // AppClose(ReservationId);
                });
            }
            else {
                //var win = window.open();
                window.location.href = "/Doctor/PrescriptionCard?Reservationid=" + ReservationId + "&Url=" + Url;
                //window.open("/Doctor/PrescriptionCard?Reservationid=" + ReservationId);
                //AppClose(ReservationId);
            }
        }
    });
}
//function AppClose(ReservationId) {
//    $.post("/Product/GetDataOfLanguage?Text='Do You want to Close Appointment?'&id=3", function (data) {
//        $(".loader").hide();
//        $("#PrintBody").html(data);
//        $("#PrintFooter").html("<div class=\"text-right margin-bottom10\"><input type=\"button\" class=\"btn btn-green\" id=\"YesPrintPre\" value=\"Yes\"><input type=\"button\" class=\"btn btn-grey\" id=\"NoPrintPre\" value=\"No\"></div>")
//        $('#Print_PopUp').modal("show");
//        ////$('.loader').hide();
//        $("body").delegate("#YesPrintPre,#NoPrintPre,#PrintPopupClose", "click", function () {
//            var IdClicked = $(this).prop("id");
//            if (IdClicked == "YesPrintPre") {
//                var win = window.open();
//                win.location = "/Doctor/DoctorInvoiceDisplay?Reservationid=" + ReservationId;
//                //window.open("/Doctor/DoctorInvoiceDisplay?ReservationId=" + ReservationId);
//                $.post("/Reservation/StatusFinalChange?Id=" + ReservationId + "&Status=C&checkornot=Checked", function () { });
//            }
//            $('#Print_PopUp').modal("hide");
//            window.location.href = "/Reservation/Reservation#openlistview";
//        });

//        //var AppCloseConfirm = confirm(data);
//        //if (AppCloseConfirm) {
//        //    window.open("/Doctor/DoctorInvoiceDisplay?ReservationId=" + ReservationId);
//        //    $.post("/Reservation/StatusFinalChange?Id=" + ReservationId + "&Status=C&checkornot=Checked", function () { });
//        //}
//        //window.location.href = "/Reservation/Reservation#openlistview";
//    });
//}
function AppClose(ReservationId, Status, YesLangText, NoLangText, Url, AlertMsg, Bool) {
    //$.post("/Product/GetDataOfLanguage?Text='Do You want to Close Appointment?'&id=3", function (data) {
    $(".loader").hide();
    $("#PrintBody").html(AlertMsg);
    $("#PrintFooter").html("<div class=\"text-right margin-bottom10\"><input type=\"button\" class=\"btn btn-green\" id=\"YesPrintPre\" value=\"" + YesLangText + "\"><input type=\"button\" class=\"btn btn-grey\" id=\"NoPrintPre\" value=\"" + NoLangText + "\"></div>")
    $('#Print_PopUp').modal("show");
    ////$('.loader').hide();
    $("body").delegate("#YesPrintPre,#NoPrintPre,#PrintPopupClose", "click", function () {
        var IdClicked = $(this).prop("id");
        if (IdClicked == "YesPrintPre") {
            // var win = window.open();
            // win.location = "/Doctor/DoctorInvoiceDisplay?Reservationid=" + ReservationId;
            //window.open("/Doctor/DoctorInvoiceDisplay?ReservationId=" + ReservationId);
            if (Status == "1")
                window.location.href = "/Reservation/CheckInvoiceStatusAndRedirectToInvoice?Reservationid=" + ReservationId + "&Url=" + Url;
            else if (Status == "2")
                window.location.href = "/Reservation/Reservation#" + Url;
            else
                window.location.href = "/Doctor/DoctorInvoiceDisplay?Reservationid=" + ReservationId + "&Url=" + Url;

            if (Bool == "True") {
                $.post("/Reservation/StatusFinalChange?Id=" + ReservationId + "&Status=C&checkornot=Checked", function () { });
            }

        }
        else {
            $('#Print_PopUp').modal("hide");
            window.location.href = "/Reservation/Reservation#" + Url;
        }

    });

    //var AppCloseConfirm = confirm(data);
    //if (AppCloseConfirm) {
    //    window.open("/Doctor/DoctorInvoiceDisplay?ReservationId=" + ReservationId);
    //    $.post("/Reservation/StatusFinalChange?Id=" + ReservationId + "&Status=C&checkornot=Checked", function () { });
    //}
    //window.location.href = "/Reservation/Reservation#openlistview";
    //});
}
function ToolTip() {
    $('[data-toggle="popover"]').popover();
}
function Medication() {
    $("#cancelMedication").click(function () {
        $("#OpenAddmedicaments").slideUp();
    });
    $("#cancelAdvice").click(function () {
        $("#OpenAddAdvice").slideUp();
    });
    $("#AddAdviceToggle").click(function () {
        $("#OpenAddAdvice").slideToggle();
    });
    $("#AddMedicamentsToggle").click(function () {
        $("#OpenAddmedicaments").slideToggle();
    });
}
function PrintpageSetUp() {
    var AllUnchecklist = $("#PrintPageSetUp li a input[type='checkbox']:unchecked").map(function () { return this.id; }).get();
    $.ajax({
        url: "/Doctor/AddPrintPageSetup",
        type: 'POST',
        dataType: "json",
        data: { getallUncheck: AllUnchecklist.toString() },
        async: true,
        success: function () {

        }
    });
    $("#PrintPageSetUp").slideUp();
}


function hashFunForTab(Status) {
    var ab = window.location.hash.substr(1);
    $('#myTab a[href="#' + ab + '"]').tab('show');
    if ($(window).width() < 768)
        ab = ab + "-collapse";
    if (Status == "1")
        GetCustomerTabDetails("#" + ab);
    else
        GetTabDetails("#" + ab);
}

function CommonSorting(url, Id) {
    $.ajax({
        url: url,
        type: 'POST',
        async: true,
        success: function (result) {
            $("#" + Id).html(result);
            $(".loader").hide();
        },
        error: function (request, status, error) { $(".loader").hide(); },
        cache: false,
    });
}
function ScheduleInputControl() {
    $(".week-schedule-content .ChkWeek span input[type='checkbox']").click(function () {
        if ($(".week-schedule-content .ChkWeek span input[type='checkbox']:checked").length > 0) $("#DaysAdd").removeAttr("disabled");
        else $("#DaysAdd").attr("disabled", "disabled");
        var add = $(this).attr("id");
        if ($("#" + add).prop("checked")) {
            $(".week-schedule-content #1" + add).removeClass("inactive");
            $(".week-schedule-content #2" + add).removeClass("inactive");
            $(".week-schedule-content #3" + add).removeClass("inactive");
            $(".week-schedule-content #4" + add).removeClass("inactive");
            $("#1" + add).removeAttr("disabled");
            $("#2" + add).removeAttr("disabled");
            $("#3" + add).removeAttr("disabled");
            $("#4" + add).removeAttr("disabled");
            $("#1" + add).addClass("required");
            $("#2" + add).addClass("required");
            $("#3" + add).addClass("required");
            $("#4" + add).addClass("required");

        }
        if (!$("#" + add).prop("checked")) {
            $(".week-schedule-content #1" + add).addClass("inactive");
            $(".week-schedule-content #2" + add).addClass("inactive");
            $(".week-schedule-content #3" + add).addClass("inactive");
            $(".week-schedule-content #4" + add).addClass("inactive");
            $("#1" + add).attr("disabled", "disabled");
            $("#2" + add).attr("disabled", "disabled");
            $("#3" + add).attr("disabled", "disabled");
            $("#4" + add).attr("disabled", "disabled");
            $("#1" + add).removeClass("required");
            $("#2" + add).removeClass("required");
            $("#3" + add).removeClass("required");
            $("#4" + add).removeClass("required");
        }
    });
}
function EditorTinyMCEForDiscussion(editors) {
    var selectors = "";
    for (i = 0; i < editors.editorControlId.length; i++) {
        if (i == 0) selectors = "#" + editors.editorControlId[i];
        else selectors += ",#" + editors.editorControlId[i];
    }
    tinyMCE.init({
        selector: selectors,
        fontsize_formats: "8pt 9pt 10pt 11pt 12pt 13pt 14pt 15pt 16pt 17pt 18pt 24pt 36pt",
        theme: "modern",
        menubar: false,
        statusbar: false,
        height: "215",
        forced_root_block: false,
        force_p_newlines: false,
        remove_linebreaks: false,
        force_br_newlines: false,
        remove_trailing_nbsp: false,
        verify_html: false,
        paste_data_images: true,
        toolbar: "image,paste",
        plugins: "image,paste",
        menubar: "insert,edit",
        setup: function (editor) {
            editor.on('change', function () {
                tinyMCE.triggerSave();
            });
        },
        relative_urls: 'true',
        remove_script_host: 'true',
        document_base_url: '/',
        convert_fonts_to_spans: false, convert_fonts_to_spans: false,
        plugins: ["advlist autolink lists link anchor", "searchreplace visualblocks code fullscreen", "insertdatetime media contextmenu paste", "placeholder"],		        // plugins: ["advlist autolink lists link anchor", "searchreplace visualblocks code fullscreen", "insertdatetime media contextmenu paste", "image", "code", "preview", "media", "imagetools", "spellchecker", "placeholder", "autoresize", "emoticons template  textcolor colorpicker textpattern imagetools"],
        toolbar: "bold italic underline | alignleft aligncenter alignright | bullist numlist | link | blockquote", plugins: ["autolink lists link anchor searchreplace visualblocks code  fullscreen insertdatetime media contextmenu  image preview imagetools spellchecker placeholder autoresize emoticons template  textcolor colorpicker textpattern "],
        toolbar1: "insertfile undo redo | bold italic underline strikethrough superscript subscript | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link | image | code | preview | imagetools | spellchecker",//| media 
        toolbar2: " forecolor backcolor emoticons | sizeselect  |  fontselect |  fontsizeselect",
        media_live_embeds: true,
        media_poster: false,
        media_alt_source: false,
        file_browser_callback: function (field_name, url, type, win) {
            $('.mce-btn.mce-open').parent().find('.mce-textbox').val("");
            if (type == 'image') $('#my_form input').click();
            if (type == 'media') $('#my_form_upload_video input').click();
        },
        browser_spellcheck: true,
        theme_advanced_toolbar_location: "top",
        theme_advanced_toolbar_align: "left",
        theme_advanced_statusbar_location: "bottom",
        auto_resize: true,
        autoresize_bottom_margin: 1,
        autoresize_min_height: 215

    });

}
function replica(id) {
    $(id).change(function () {
        var Element = $(this);
        var getReplicationId = "";
        getReplicationId = "#" + Element.attr("replica");
        if (getReplicationId != "" && getReplicationId != null && getReplicationId != undefined) {
            if (Element.attr("type") == "text" || Element.attr("type") == "number")
            { $(getReplicationId).val(Element.val()); }
            if (Element.is("select"))
            { $(getReplicationId).val(Element.val()); }
            if (Element.is("textarea"))
            { $(getReplicationId).val(Element.val()); }
        }
    });
}
function ExporttoExcel(Table) {
    $("#" + Table).tableExport({
        type: "excel", escape: false, htmlContent: false
    });
}
function ShowHideCustomSettlementTextBox(Key, Id, value, DisplayId) {
    $("#" + DisplayId).val(value)
    if (Key == 1)
        $("#" + Id).removeClass("display-none");
    else
        $("#" + Id).addClass("display-none");
}
function RedirectToTab(LinkName) {
    if (window.location.toString().indexOf('#') != -1) {
        var ab = window.location.hash.substr(1);
        if (ab == LinkName) {
            location.reload();
        }
        else {
            window.location.hash = LinkName;
            window.location.reload();
        }
    }
    else {
        window.location.hash = '#' + LinkName;
        window.location.reload();
    }
}
function GetViewofNote(Event) {
    var BookingId = Event.attr("BookingId");
    if (BookingId != "" && BookingId != undefined) {
        $(".loader").show();
        Event.parents(".Note_date").next().load("/Doctor/DisplayNotes?Reservationid=" + BookingId, function () {
            $(".loader").hide();
        });
    }
}
function PrintNotesHistory(Event) {
    $(".loader").show();
    var HtmlContent = $.trim(Event.parents(".Note_date").next().html());
    if (HtmlContent != "") {
        $("#print-Region").html(HtmlContent);
        $(".loader").hide();
        window.print();
    }
    else {
        var BookingId = Event.attr("BookingId");
        if (BookingId != "" && BookingId != undefined) {
            Event.parents(".Note_date").next().load("/Doctor/DisplayNotes?Reservationid=" + BookingId, function () {
                $("#print-Region").html(Event.parents(".Note_date").next().html());
                $(".loader").hide();
                window.print();
            });
        }
    }
}

function QuickBlockEditDelete() {
    $("body").delegate(".Quick_Edit .fa-trash,.Quick_Edit .fa-pencil-square-o", "click", function () {
        if ($(this).closest(".fa-trash").length > 0)
            CancelCalBookedDetails($(this).attr("ResId"), "QuickBlocker", $(this).attr("BookingDate"));
        else {
            $(".loader").show();
            var Calview = $("#calendarmonth .view-cal .active").prop("id");
            var ViewStatus = "";
            if (Calview == "DayBTN")
                ViewStatus = "Day";
            if (Calview == "WeekBTN")
                ViewStatus = "Week";
            if (Calview == "MONTHBTN")
                ViewStatus = "Month";
            var AllView = $("#EmployeesDaybtn .activebutton").prop("id");
            if (AllView == "All" && ViewStatus == "Day")
                ViewStatus = "All";
            var ResId = $(this).attr("ResId");
            var UserId = $(this).attr("UserId")
            $.post("/Reservation/QuickBlockPopup?UserId=" + UserId + "&Status=" + ViewStatus + "&ReservationId=" + ResId, function (BlockDetails) {
                $("#WelcomeAlert").html("<h1 class=\"text-center margin-bottom30\"><span>CLICK-AND-GO!</span></h1><p class=\"title15 text-center margin-bottom30\">" + BlockDetails + "</p>");
                $('#welcome').modal("show");
                $(".loader").hide();
            });
        }
    });
}
function AutocompleteDeatils(Id, GetDetailName, Type) {
    var input = document.getElementById(Id);
    var autocomplete = new google.maps.places.Autocomplete(input, { types: [Type] });
    google.maps.event.addListener(autocomplete, GetDetailName, function () {
        var place = autocomplete.getPlace();
        $("#" + Id).val(place.name);
    });
}

function RemoveAllHeights() {
    $("#ShowReport").slideToggle('slow').html("").unbind().slideToggle();
    $("#ImportHeight").addClass("Import_height");
}
function AppointMentClosed(Id, ChkId, RedirectLink, View, AllView, Date, EmpUserId) {
    var stchecked = "";
    if ($("#" + ChkId).prop("checked"))
        stchecked = "Checked";
    else
        stchecked = "Not";
    var Url = "/Reservation/BookingStatusChange?Id=" + Id + "&Status=C&checkornot=" + stchecked + "&View=" + View + "&AllView=" + AllView + "&Date=" + Date + "&EmpUserId=" + EmpUserId
    CommonStatusChangeFun("Do You want to Close Appointment?", "3", Url, RedirectLink, "#" + ChkId);
}
function CommonStatusChangeFun(LangTxt, LangTxtId, Url, RedirectLink, LinkId) {
    $.post("/Product/GetDataOfLanguage?Text='" + LangTxt + "'&id=" + LangTxtId, function (data) {
        $(".loader").hide();
        var confirmClose = confirm(data);
        if (confirmClose) {
            $(".loader").show();
            $.post(Url, function () {
                RedirectToTab(RedirectLink);
            });
        }
        else
            $(LinkId).prop("checked", false);
    });
}








