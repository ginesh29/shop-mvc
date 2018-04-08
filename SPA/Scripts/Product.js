function UpdateSettlement(obj,SuccessLangTxt,ErrorLangTxt) {
    var DataArray = [];
    var data = [];
    var Access = true;
    var status = "success";
    if (obj == null || obj == undefined)
        data = ($("#AssignedSettlementTB tbody tr:not(:hidden)"));
    else
        data.push(obj);
    $(data).each(function () {
        var objects = $(this);
        if ((objects.find("input.Duration").val().indexOf(".") != -1 || !isInteger(objects.find("input.Duration").val(),4,null,null,0) || !isFloat(objects.find("input.Amount").val(),null,9999.99,null,0)) && Access == true) {
            objects.find("input.Duration,input.Amount").addClass("border-red").delay(2000).queue(function () {objects.find("input.Duration,input.Amount").removeClass("border-red").clearQueue() });
            Access = false;
            return Access;
        }
        var Array = {
            Health_Assign_id: $(this).attr("id"),
            Duration: $(this).find("input.Duration").val(),
            Amount: $(this).find("input.Amount").val()
        }
        DataArray.push(Array);
    });
    if (Access) {
        $.ajax({
            type: "POST",
            url: "/Product/AddMissingInformation",
            data: '{settleTxt: ' + JSON.stringify(DataArray) + '}',
            contentType: "application/json; charset=utf-8",
            success: function (response) {
                if (response)
                    ShowPopup(SuccessLangTxt, status, true);
                else
                    ShowPopup(ErrorLangTxt, "error", true);
            },
            error: function (response) {
                ShowPopup(ErrorLangTxt, "error", true);
            },

        });
    }

}
function DeleteSettlement(obj,SuccessLangTxt,ErrorLangTxt) {
    var id = obj.attr("id");
    var InsuranceId = obj.attr("insuranceId");
    $.ajax({
        type: "POST",
        url: "/Product/DeleteAssignedSettlement?Id=" + id,
        success: function (response) {
            if (response) {
                obj.remove();
                if ($("#AssignedSettlementTB tr:not(':hidden')").length == 0) {
                    $("#AssignedSettlementTB tbody tr:last").show();
                    $("#UpdateallStmt").hide();
                    $("AllSettlement #display_" + InsuranceId + " input[type='check']").attr("checked", false);
                }
                ShowPopup(SuccessLangTxt, "success", true);
            }
            else
                ShowPopup(ErrorLangTxt, "error", true);
        },
        error: function (response) {
            ShowPopup(ErrorLangTxt, "error", true);
        }
    });

}