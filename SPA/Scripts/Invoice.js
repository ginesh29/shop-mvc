$.ajaxSetup({ statusCode: { 401: function () { window.location.href = "/Login/SignIn"; } }, async: true });
$("#Forinvoicing .InvoiceCreate,#Forinvoicing .ManualInvoice").click(function () {
    var ReservationList = "";
    var GetCurrent = $(this);
    var Status = "A";
    $(this).parents(".parent:first").find("input[type='checkbox'][name='ReservationList']:checked").each(function () {
        if (GetCurrent.parents(".parent:first").find("input[type='checkbox'][name='ReservationList'][Identification='" + $(this).attr("Identification") + "']:checked").length <= 15) {
            if (ReservationList == "") {
                ReservationList = this.id;
            }
            else {
                ReservationList = ReservationList + "," + this.id;
            }
        }
        else {
            $("#WelcomeAlert").html("<h1 class=\"text-center margin-bottom30\"><span>CLICK-AND-GO!</span></h1><p class=\"title15 text-center margin-bottom30\">" + $("#FIErrorMsg").val() + "</p>");
            $('#welcome').modal("show");
            Status = "AB";
            return false;
        }
    });
    if (ReservationList == "" && Status != "AB") {
        OpenCreateManualIPopup("Forinvoicing");
    }
    if (ReservationList != "" && Status != "AB") {
        window.location.href = "/Invoice/Invoice1?ReservationIdList=" + ReservationList + "&Url=Forinvoicing";
    }
});
function OpenCreateManualIPopup(UrlStatus) {
    $.ajax({
        url: "/Invoice/CreateManualInvoice?UrlStatus=" + UrlStatus,
        beforeSend: function () {
            if (!$(".loader").is(":visible"))
                $(".loader").show();
        },
        type: 'POST',
        async: true,
        success: function (data) {
            $("#ManualInvoicePopup").html("").html(data);
            $('#OpenCreateManualInvoicePopup').modal('show');
        },
        complete: function () {
            $(".loader").hide();
        },
        cache: false,
        contentType: false,
        processData: false
    });
}

$("#InvoiceGeneratedPage .AllIGPrintSend").click(function () {
    var Allcheck = $(this).parents(".parent:first").find("input[type='checkbox']:checked").map(function () { return $(this).attr("invoice_id") }).get().toString();
    if (Allcheck != "") {
        window.location.href = "/Invoice/MailPrintMultiple?InvoiceId=" + Allcheck + "&url=InvoiceGenerated";
    }
    else {
        var Message = $("#IGErrorMsg").val();
        $("#WelcomeAlert").html("<h1 class=\"text-center margin-bottom30\"><span>CLICK-AND-GO!</span></h1><p class=\"title15 text-center margin-bottom30\">" + Message + "</p>");
        $('#welcome').modal("show");
    }
});
$("#SavePayRecord").click(function () {
    var Addvalidate = $("#frmPartialPayment").validate().form();
    if (Addvalidate != false) {
        var formData = new FormData($("#frmPartialPayment")[0]);
        $.ajax({
            url: "/Invoice/CompleteInvoice",
            beforeSend: function () {
                if (!$(".loader").is(":visible"))
                    $(".loader").show();
            },
            type: 'POST',
            data: formData,
            async: true,
            success: function (Info) {
                // var Message = "Failed to complete the payment procedure";
                if (Info.UrlStatus == "OpenInvoices") {
                    var Message = $("#OIErrorMsg").val();
                    if (Info.Msg == "Success") {
                        // Message = "Successfully Payment procedure is completed";
                        Message = $("#OISuccessMsg").val();
                        CommonTabDetails('/Invoice/OpenInvoices', '#OpenInvoices');
                    }
                    $('#CloseandPay').modal("hide");
                    $("#WelcomeAlert").html("<h1 class=\"text-center margin-bottom30\"><span>CLICK-AND-GO!</span></h1><p class=\"title15 text-center margin-bottom30\">" + Message + "</p>");
                    $('#welcome').modal("show");
                }
                else
                    window.location.href = "/Invoice/PayAndClosedInvoice?InvoiceId=" + Info.Invoice_Id + "&Invoice_Service=" + Info.Invoice_Service + "&Url=" + Info.UrlStatus + "&Msg=" + Info.UrlStatus + "_PCMP&FullPayment=" + Info.FullPayment;

            },
            complete: function () {
                $(".loader").hide();
            },
            cache: false,
            contentType: false,
            processData: false
        });

    }
    else { $('.loader').hide(); }
});
function OpenPartialpopup(InvoiceId, LangId, LngLocal, ShopDate, Url, Invoice_Service) {
    $.ajax({
        url: "/Invoice/PartialPaymentPopup?InvoiceId=" + InvoiceId + "&LangId=" + LangId + "&LangLocal=" + LngLocal + "&ShopDate=" + ShopDate + "&UrlStatus=" + Url + "&Invoice_Service=" + Invoice_Service,
        beforeSend: function () {
            if (!$(".loader").is(":visible"))
                $(".loader").show();
        },
        type: 'POST',
        async: true,
        success: function (data) {
            $("#ShowPartialPaymentPopup").html("").html(data);
            $('#CloseandPay').modal("show");

        },
        complete: function () {
            $(".loader").hide();
        },
        cache: false,
        contentType: false,
        processData: false
    });
}
function InitDateTimePicker(dateRange) {
    // var date = new Date();
    // var minDate = new Date();
    // minDate.setDate(date.getDate() - 1);
    $('#' + dateRange.dateStartDateControlId).on("dp.change", function (e) {
        //  $('#' + dateRange.dateStartDateControlId).data("DateTimePicker").minDate(minDate);
        $('#' + dateRange.dateStartDateControlId).data("DateTimePicker").format('DD/MM/YYYY');
    });
    $('#' + dateRange.dateStartDateControlId).datetimepicker({

    });
    $('#' + dateRange.dateStartDateControlId).on("dp.change", function (e) {
        $('#' + dateRange.dateEndDateControlId).data("DateTimePicker").minDate(e.date);
        $('#' + dateRange.dateEndDateControlId).data("DateTimePicker").format('DD/MM/YYYY');

    });
    $('#' + dateRange.dateEndDateControlId).datetimepicker({
    });
}
function DeleteInvoice(InvoiceId, UrlPath, TabId) {
    $.ajax({
        url: "/invoice/DeleteInvoice?InvoiceId=" + InvoiceId,
        method: "post",
        success: function () {
            CommonTabDetails(UrlPath, TabId);
            $(".loader").hide();
        }
    })

}
