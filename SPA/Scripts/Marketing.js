$("#selectContent li a input[type='checkbox']").click(function () {
    var a = $(this).prop("id");
    $.each($("#selectContent li a input[type='checkbox']"), function () {
        if ($(this).prop("checked") && $(this).prop("id") != a) { $(this).prop("checked", false); }
    });
    if ($(this).prop("checked") == true) {
        var id = $("#MCustomerList li a input[type='checkbox']:checked").prop("id");
        $("#" + id).click();
    }
});
$("#MCustomerList li a input[type='checkbox']").click(function () {
    if ($("#selectContent").has("li").length > 0 && $('#selectContent li a input:checkbox:checked').length > 0) {
        var id = $(this).prop("id");
        $.each($("#MCustomerList li a input[type='checkbox']"), function () {
            if ($(this).prop("checked") && $(this).prop("id") != id) { $(this).prop("checked", false); }
        });
        $("#ChkAllCustomer").prop("checked", false);
        var TextStatus = $('#selectContent li a input:checkbox:checked').attr("Status");
        if ($("#" + $(this).prop("id")).prop("checked")) {
            $.post("/Shop/MCustomerDetails?Status=" + id + "&TxtStatus=" + TextStatus, function (Data) {
                $("#ShowCustomerList").show();
                $("#ConfirmAndSend").show();
                window.scrollTo(0, document.body.scrollHeight);
                $("#MCustomerDetails").html(Data);
            });
        }
        else {
            window.scrollTo(0, 0);
            ResetCustomerList();
        }
    }
    else {
        if ($(this).prop("checked") == true) {
            $(this).prop("checked", false);
            $("#selectContent").addClass("ContentRed").delay(2000).queue(function () { $("#selectContent").removeClass("ContentRed").clearQueue() });
        }
        ResetCustomerList();
    }

});
function ResetCustomerList() {
    $("#ShowCustomerList").hide();
    $("#ConfirmAndSend").hide();
    $("#MCustomerDetails").html("");
    $("#ChkAllCustomer").prop("checked", false);
    $("#CustomerListcount").html("");
}

$("#ChkAllCustomer").click(function () {
    if ($("#" + $(this).prop("id")).prop("checked")) {
        $.each($("#MCustomerDetails tr td input[type='checkbox']"), function () {
            $(this).prop("checked", true);
        });
    }
    else {
        $.each($("#MCustomerDetails tr td  input[type='checkbox']"), function () {
            $(this).prop("checked", false);
        });
    }
});
function SendNewsToCustomer() {
    var UseridList = $("#MCustomerDetails tr td input[type='checkbox']:checked").map(function () { return this.id; }).get();
    if (UseridList.length > 0) {
        $(".loader").show();
        var contentId = $("#selectContent li a input[type='checkbox']:checked").prop("id").replace("Ak_", "");
        $.post("/Shop/SendDetails?UserList=" + UseridList + "&ContentId=" + contentId, function () {

            if (window.location.toString().indexOf('#') != -1) {
                var ab = window.location.hash.substr(1);
                if (ab == "Various") { location.reload(); }
                else {
                    window.location.hash = 'Marketing';
                    window.location.reload();
                }
            }
            else {
                window.location.hash = '#Marketing';
                window.location.reload();
            }
            $(".loader").hide();
        });
    }
    else {
        $("#SelectCustomerBorder").addClass("ContentRed").delay(2000).queue(function () { $("#SelectCustomerBorder").removeClass("ContentRed").clearQueue() });
    }
}
function EditMarketingData(MID, Status) {
    if (Status == "SMS")
        $("a[href='#smssetup']").click();
    else
        $("a[href='#emailsetup']").click();
    $.ajax({
        url: "/Shop/EditMarketingData?MID=" + MID,
        type: 'POST',
        async: true,
        dataType: "json",
        success: function (EditDetails) {
            if (EditDetails.status == "SMS") {
                $("#SMID").val(EditDetails.MID);
                $("#Subject").val(EditDetails.Subject);
                $("#content").val(EditDetails.content);
            }
            else {
                $("#EMID").val(EditDetails.MID);
                $("#ESubject").val(EditDetails.Subject);
                tinymce.get('TextMarketing').setContent(EditDetails.content);
                $("#TextMarketing").val(EditDetails.content);
                // Edit DropZone
                //$("#MarketingObject").val(EditDetails.Extra_1)
                //alert(EditDetails.Extra_1);
                //$.each(EditDetails.Extra_1, function (index, item) {
                //    alert(item);
                //    var mockFile = {
                //        name: item.replace("/MarketingUpload/",""),
                //        size: 12345,
                //        status: Dropzone.ADDED,
                //        url: item
                //    };
                //    alert(mockFile.name);
                //    $("#MarketingFileUpload").emit("addedfile", mockFile.name);
                //    $("#MarketingFileUpload").emit("thumbnail", mockFile.name, item);
                //});
                // endcode
            }
        }
    });
}
$("#SMSFORM_cancel").click(function () {
    $("#SMSFORM")[0].reset();
    $("#SMSFORM").validate().resetForm();
    $("#SMID").val("");
    $("#RemainingChar").text("");
});
$("#EMAILFORM_cancel").click(function () {
    $("#EMAILFORM")[0].reset();
    $("#EMAILFORM").validate().resetForm();
    $("#EMID").val("");
});
function onchangefileupload() {
    var formData = new FormData();
    var totalFiles = document.getElementById("Uploadmedia").files.length
    for (var i = 0; i < totalFiles; i++) {
        var file = document.getElementById("Uploadmedia").files[i];
        formData.append("Uploadmedia", file);
    }
    $.ajax({
        type: "POST",
        url: '/Shop/UploadImage',
        data: formData,
        contentType: false,
        processData: false,
        success: function (response) {
            if (response != "0")
                $('.mce-btn.mce-open').parent().find('.mce-textbox').val(response);
            else
                $('.mce-btn.mce-open').parent().find('.mce-textbox').val("");
        }
    });
}
function AddMarket(ab, LangText) {
    var Chkvalidation = false;
    if (ab == "SMSFORM") {
        Chkvalidation = $("#" + ab).validate({
            rules: { content: { maxlength: 150 } }
        }).form();
    }
    else
        Chkvalidation = $("#" + ab).validate().form();
    if (Chkvalidation != false) {
        if (ab == "SMSFORM" || (ab == "EMAILFORM" && tinyMCE.get('TextMarketing').getContent() != "")) {
            //if (ab == "SMSFORM" && $("#RemainingChar").val() <= 150) {
            $(".loader").show();
            var formData = new FormData($("#" + ab)[0]);
            $.ajax({
                url: $("#" + ab).attr("action"),
                async: true,
                type: 'POST',
                dataType: "json",
                data: formData,
                async: true,
                success: function (details) {
                    $(".loader").hide();
                    $("#" + ab)[0].reset();
                    var Symbolclass = "";
                    if (details.status == "SMS")
                        Symbolclass = "fa fa-commenting";
                    else
                        Symbolclass = "fa fa-envelope";
                    var DisplayHtml = "<li> <a class='doctor-checkbox'><input type='checkbox' id='Ak_" + details.MID + "'><label class='dropmenuEllips' for='Ak_" + details.MID + "'>" + details.Subject + "</label><span class='marketgin-mail'><i class='" + Symbolclass + "' aria-hidden='true'></i></span><span class='marketing-edit' onclick=EditMarketingData(" + details.MID + ",'" + details.status + "');><i class='fa fa-pencil-square-o' data-trigger='hover'></i></span><span class='marketing-detele' id='" + details.MID + "'><i class='fa fa-trash' data-toggle='popover' data-trigger='hover' data-placement='top' data-content='Delete' data-original-title='' title=''></i></span></a></li>";
                    if (details.Chkstatus == "U") {
                        $("#selectContent li a span[id='" + details.MID + "']").closest("li").find("label").html(details.Subject);
                    }
                    else {
                        var Length = $("#selectContent li").length;
                        if (Length == 0)
                            $("#selectContent").html(DisplayHtml)
                        else
                            $("#selectContent").find("li").first().before(DisplayHtml);
                    }
                    $('div.dz-success').remove();
                    $("#RemainingChar").text("");
                },
                cache: false,
                contentType: false,
                processData: false
            });
            return false;
            //}
            //else
            //    $("#SmsTxtareaStatus").text("Please enter less or equal to 150 characters").fadeIn().delay(2000).queue(function () { $("#SmsTxtareaStatus").text("").clearQueue() });
        }
        else
            $("#EmailTxtareaStatus").text(LangText).fadeIn().delay(2000).fadeOut();
    }

}
$("body").delegate("#selectContent li a .marketing-detele", "click", function () {
    var Id = $(this).prop("id");
    var closestliRemove = $(this).closest("li").remove();
    $.ajax({
        url: "/Shop/DeleteMarketing",
        method: "post",
        data: { id: Id },
        success: function () { }
    })
});