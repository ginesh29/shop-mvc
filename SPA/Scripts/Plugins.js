//Datetimepicker
function Dob(Id, locale, mindate, maxdate, widgetPositioning) {
    var date = new Date();
    date.setDate(date.getDate());
    var Obj = {};
    Obj.format = 'DD-MM-YYYY';
    Obj.locale = locale;
    if (maxdate == true)
        Obj.maxDate = date;
    if (mindate == true)
        Obj.minDate = moment().subtract(200, 'years');
    if (widgetPositioning == true) {
        Obj.widgetPositioning = {
            horizontal: 'auto',
            vertical: 'top'
        }
    }
    $(Id).datetimepicker(Obj);
}
function CommonDatePicker(Id, locale, mindate, maxdate, widgetPositioning) {
    var date = new Date();
    date.setDate(date.getDate());
    var Obj = {};
    Obj.format = 'DD-MM-YYYY';
    Obj.locale = locale;
    if (maxdate == true)
        Obj.maxDate = date;
    if (mindate == true)
        Obj.minDate = date;
    if (widgetPositioning == true) {
        Obj.widgetPositioning = {
            horizontal: 'auto',
            vertical: 'top'
        }
    }
    $(Id).datetimepicker(Obj);
}
//Message Popup Function
function ShowPopup(Message, status, ShowButton) {
    swal({
        text: Message,
        title: 'CLICK-AND-GO',
        type: status,
        showConfirmButton: ShowButton,
        showCloseButton: true
    });
}
//Validation for is Float
function isFloat(n, Lengthvalidate, valuevalidate, LengthMinValidate, valueMinValidate) {
    var rtun = !isNaN(parseFloat(n)) && $.isNumeric(n);
    if (Lengthvalidate != undefined && Lengthvalidate != null && rtun == true)
        rtun = n.length <= Lengthvalidate;
    if (LengthMinValidate != undefined && LengthMinValidate != null && rtun == true)
        rtun = n.length >= Lengthvalidate;
    if (valuevalidate != undefined && valuevalidate != null && rtun == true)
        rtun = parseFloat(n) <= valuevalidate;
    if (valueMinValidate != undefined && valueMinValidate != null && rtun == true)
        rtun = parseFloat(n) >= valueMinValidate;
    return rtun;
}
//Validation for Integer
function isInteger(n, Lengthvalidate, valuevalidate, LengthMinValidate, valueMinValidate) {
    var rtun = !isNaN(parseInt(n)) && $.isNumeric(n);
    if (Lengthvalidate != undefined && Lengthvalidate != null && rtun == true)
        rtun = n.length <= Lengthvalidate;
    if (LengthMinValidate != undefined && LengthMinValidate != null && rtun == true)
        rtun = n.length >= Lengthvalidate;
    if (valuevalidate != undefined && valuevalidate != null && rtun == true)
        rtun = parseInt(n) <= valuevalidate;
    if (valueMinValidate != undefined && valueMinValidate != null && rtun == true)
        rtun = parseInt(n) >= valueMinValidate;
    return rtun;
}
//Search in selected List
function SearchInSelectedList(IdList) {
    $(IdList).keyup(function () {
        var SearchText = $.trim($(this).val().toLowerCase());
        var Parent = $(this).parent().parent();
        var Child = Parent.find("ul").first();
        var Array = Child.find("li");
        var cnt = 1;
        Array.each(function () {
            var FindingString = "label";
            if (Child.hasClass("dropdown-menu") || Child.hasClass("dropdown-menus"))
                FindingString = "a";
            var Result = $(this).find(FindingString).text().toLowerCase().indexOf(SearchText) > -1;

            if (Result)
                $(this).removeClass("display-none");
            else
                $(this).addClass("display-none");
            //if want add then use this code
            if (Array.length == cnt) {
                if (Child.find("li[class!='display-none']").length == 0) {
                    Child.addClass("display-none");
                    Parent.find(".NewAddlist").removeClass("display-none");
                }
                else {
                    Child.removeClass("display-none");
                    if (!Parent.find(".NewAddlist").first().hasClass("display-none"))
                        Parent.find(".NewAddlist").first().addClass("display-none");
                }
            }
            cnt = cnt + 1;
        });
        if (Array.length == 0)
            Parent.find(".NewAddlist").removeClass("display-none");
    });

}
//Search in Table
function searchInPage(SearchField, Table, gt, lt) {
    $("#" + SearchField).keyup(function () {
        var SearchText = $(this).val().toLowerCase();
        var Array = $("#" + Table + " tbody tr").not(".display-none");
        Array.each(function () {
            var MainParent = $(this);
            var Row = $(this).find("td:gt(" + gt + "):lt(" + lt + ")").map(function () { if ($(this).html().toLowerCase().indexOf(SearchText) > -1) { return $(this).html(); } }).get().length;
            if (Row > 0)
                MainParent.show();
            else
                MainParent.hide();
        })
    });
}
//Tab Functions
function GetCustomerTabDetails(InitialName) {
    $(".loader").show();
    if (InitialName.indexOf("listview") >= 0)
        url = "/Customer/ListView";
    if (InitialName.indexOf("newcustomer") >= 0)
        url = "/Customer/NewCustomer";
    if (InitialName.indexOf("blacklist") >= 0)
        url = "/Customer/BlackList";
    if (InitialName.indexOf("TopTenCustomer") >= 0)
        url = "/Customer/TopTenCustomer";
    if (InitialName.indexOf("Import") >= 0)
        url = "/Customer/Import";
    CommonTabDetails(url, InitialName);
}
function GetTabDetails(InitialName) {
    $(".loader").show();
    var url = "";
    if (InitialName.indexOf("pendingforapproval") >= 0)
        url = "/Reservation/PendingforApproval";
    if (InitialName.indexOf("openlistview") >= 0)
        url = "/Reservation/OpenListView?Status=Pending";
    if (InitialName.indexOf("Appclosed") >= 0)
        url = "/Reservation/OpenListView?Status=Appointment Closed";
    if (InitialName.indexOf("OpenListDeclined") >= 0)
        url = "/Reservation/OpenListView?Status=Declined";
    if (InitialName.indexOf("calendarmonth") >= 0)
        url = "/Reservation/CalendarMonth";
    if (InitialName.indexOf("HolidayCalendar") >= 0)
        url = "/Reservation/HolidayCalendar";
    if (InitialName.indexOf("shopownersetup") >= 0)
        url = "/Shop/ShopOwnerSetup";
    if (InitialName.indexOf("holiday") >= 0)
        url = "/Shop/Holiday";
    if (InitialName.indexOf("texts") >= 0)
        url = "/Shop/Texts";
    if (InitialName.indexOf("changepassword") >= 0)
        url = "/Shop/ChangePassword";
    if (InitialName.indexOf("Various") >= 0)
        url = "/Shop/Various";
    if (InitialName.indexOf("Language") >= 0)
        url = "/Shop/Language";
    if (InitialName.indexOf("Marketing") >= 0)
        url = "/Shop/Marketing";
    if (InitialName.indexOf("Lending") >= 0)
        url = "/Shop/LendingPage";
    if (InitialName.indexOf("Payment") >= 0)
        url = "/Shop/Payment";
    if (InitialName.indexOf("TimeSlot") >= 0)
        url = "/Shop/Timeslot";
    if (InitialName.indexOf("PaymentHistory") >= 0)
        url = "/Shop/PaymentHistory";
    if (InitialName.indexOf("Roles") >= 0)
        url = "/AccessRight/Roles";
    if (InitialName.indexOf("Permissions") >= 0)
        url = "/AccessRight/Permissions";
    if (InitialName.indexOf("Forinvoicing") >= 0)
        url = "/Invoice/Forinvoicing";
    if (InitialName.indexOf("InvoiceGenerated") >= 0)
        url = "/Invoice/InvoiceGenerated";
    if (InitialName.indexOf("OpenInvoices") >= 0)
        url = "/Invoice/OpenInvoices";
    if (InitialName.indexOf("ClosedInvoices") >= 0)
        url = "/Invoice/ClosedInvoices";
    if (InitialName.indexOf("Reports") >= 0)
        url = "/Invoice/Reports";
    if (InitialName.indexOf("Product_Catalog") >= 0)
        url = "/Product/Product";
    if (InitialName.indexOf("AddOnProduct") >= 0)
        url = "/Product/AddOnProduct";
    if (InitialName.indexOf("SettlementText") >= 0)
        url = "/Product/SettlementText";
    CommonTabDetails(url, InitialName);
}
function CommonTabDetails(url, InitialName) {
    InitialName = InitialName + "," + InitialName + "-collapse";
    $.ajax({
        url: url,
        type: 'GET',
        async: true,
        beforeSend: function () {
            if (!$(".loader").is(":visible"))
                $(".loader").show();
            $(InitialName).closest(".tab-content").find(".tab-pane").html("");
        },
        success: function (result) {
            $(InitialName).html(result);
        },
        complete: function () {
            $(".loader").hide();
        },
        error: function (request, status, error) { $(".loader").hide(); },
        cache: false,
    });
}
