/*All timepicker will work as per the day is active or not in Employee week schedule*/
$("#days_list li input[type='checkbox']").change(function () {
    var order = $(this).parents("li:first").index() + 1;
    $("ul.time_section ul.time_box li:nth-child(" + order + ") input[type='text']").prop("disabled", !$(this).prop("checked"));
});
function NewLaneCheck(current, cell, Row) {
    var getpastdate = $("ul.time_section li.time_section-li:lt(" + (cell) + ") ul.time_box li:nth-child(" + Row + ") input[type='text']").filter(function () {
        if ($(this).val() != '') {
            return $(this);
        }
    }).last();
    //current Datetimepicker logic
    if (getpastdate.length > 0 && cell != 0) {
        if (getpastdate.val() != "" && getpastdate.val() != null && current.attr("done") != "true") {
            current.data("DateTimePicker").minDate(getpastdate.val());
            current.val(getpastdate.val());
            current.attr("done", "true");
        }
    }
}
$('.week-time input[type="Text"]').on("dp.show", function (e) {

    $(this).parents("ul").find("script").remove();
    var cell = $(this).parents(".time_section-li:first").index();
    var Row = $(this).parents(".week-time:first").index() + 1;
    NewLaneCheck($(this), cell, Row);
})
/*time picker validation*/
$('.week-time input[type="Text"]').on("dp.change", function (e) {
    $(this).parents("ul").find("script").remove();
    var cell = $(this).parents(".time_section-li:first").index();
    var Row = $(this).parents(".week-time:first").index() + 1;
    var currents = $(this);
    NewLaneCheck($(this), cell, Row);
    //All the Datetimepicker after the current one selected
    if ($("ul.time_section li.time_section-li:gt(" + cell + ") ul.time_box li:nth-child(" + Row + ") input[type='text']").length > 0) {
        $("ul.time_section li.time_section-li:gt(" + cell + ") ul.time_box li:nth-child(" + Row + ") input[type='text']").each(function () {
            if (currents.data("DateTimePicker").date() >= currents.data("DateTimePicker").minDate())
                $(this).val(currents.data("DateTimePicker").date().format("HH:mm")).data("DateTimePicker").minDate(currents.data("DateTimePicker").date().format("HH:mm"));
            else
                $(this).val(currents.data("DateTimePicker").minDate().format("HH:mm")).data("DateTimePicker").minDate(currents.data("DateTimePicker").minDate().format("HH:mm"));
            $(this).attr("done", "true");
        });
    }
});


//function getNewLane(lbl1, lbl2) {
//Note above line should be used when breaks are considered as per timeslot
/*In Week Schedule You Can Get New Lane For Entering Time*/
function getNewLane(lbl1, lbl2, slot) {
    $.ajax({
        url: "/Employee/getDynamicLane",
        type: 'GET',
        data: {
            Label1: lbl1,
            Label2: lbl2,
            index: ($("ul.time_section li.time_section-li").length + 1),
            slot: slot
        },
        async: true,
        success: function (result) {
            $("ul.time_section").find("script").remove();
            $("#closeSectionList").find("li").last().after("<li class='weekbox_close CloseWeekBox'><i class='fa fa-times' aria-hidden='true'></i></li>");
            $("ul.time_section").append(result);
            $("#days_list input[type='checkbox']:not(:checked)").each(function () {
                var Row = $(this).parents(".ChkWeek:first").index() + 1;
                $("ul.time_box li:nth-child(" + Row + ") input[type='text']").attr("disabled", "disabled");
            })            
        },
        cache: false,
    });
}
$("body").delegate("#closeSectionList .CloseWeekBox", "click", function () {
    var Index = ($(this).index() + 1) * 2;
    $("ul.time_section li.time_section-li:eq(" + (Index - 2) + "),ul.time_section li.time_section-li:eq(" + (Index - 1) + ")").remove();
    $(this).remove();
});

