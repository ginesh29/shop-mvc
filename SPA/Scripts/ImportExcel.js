if ($('.draggable,.droppable').data('sortable')) {
    $(this).sortable("destroy");
}
$(".draggable").sortable({
    connectWith: "ul",
    revert: "invalid",
    appendTo: '.droppable',
    cursorAt: { top: 0, left: 0 },
    tolerance: "pointer",
    dropOnEmpty: true,
    scroll:true,
    start: function (event, ui) {
        InitialSort(0);
    },
    stop: function (event, ui) {
        $(".droppable").removeClass("import_active");
        $(".droppable").find(".drag_div").show();
        $(ui.item[0].parentElement).find(".drag_div[class!='hide']").hide();
         btnDynamicFields();
    },
    update: function (event, ui) {
        $(".droppable").removeClass("import_active");
        if (ui.item.hasClass("ui-state-default") && $(ui.item[0].parentElement).hasClass("droppable")) {
            if ($(ui.item[0].parentElement).find("li").not(".drag_div").length == 1) {
                $(ui.item[0].parentElement).find(".drag_div").addClass("hide").hide();
                $(ui.item[0].parentElement).find("input:hidden").val($.trim(ui.item.text()));
                $(ui.item[0].parentElement).sortable('disable');
                ui.item.replaceWith('<li class="drag-filed Dropped"><h5>' + $.trim(ui.item.text()) + '<a class="Removeli"><i class="fa fa-times-circle" aria-hidden="true"></i></a></h5></li>');
            }
            else {
                ui.item.replaceWith("");
                $(".draggable").append("<li class='ui-state-default'><span class='Field_img'></span><span class='Field_text'>" + $.trim(ui.item.text()) + "</span></li>")
            }
        }
    }
}).disableSelection();
$(".droppable").sortable({
    scroll: true
}).disableSelection();
$(".Dynamic-Field").delegate(".Removeli", "click", function () {
    var lidata = $(this).parents("li:first");
    $(".draggable").prepend("<li class='ui-state-default'><span class='Field_img'></span><span class='Field_text'>" + lidata.text() + "</span></li>")
    $(this).parents("ul:first").find(".drag_div").removeClass("hide").show();
    $(this).find("input:hidden").val("");
    $(this).parents("ul:first").sortable('enable');
    lidata.remove();
    btnDynamicFields();
});
function InitialSort(a) {
    $(".droppable").each(function () {
        if ($(this).find(".Dropped").length > 0) {
            $(this).sortable('disable');
        }
        else {
            if (a == 0)
                $(this).addClass("import_active").find(".drag_div").hide();
            $(this).sortable('enable');
        }
    })
}
InitialSort();
//function RemoveAllHeights()
//{
//    $("#ShowReport").slideToggle('slow').html("").unbind().slideToggle();
//    $("#ImportHeight").addClass("Import_height");
//}
$("#CancelBtn").click(function () {
    RemoveAllHeights();
})
function btnDynamicFields() {
    if ($(".droppable .drag-filed").length == 0)
        $("#btnAddDynamicFields").attr("disabled", "disabled");
    else
        $("#btnAddDynamicFields").removeAttr("disabled");
}