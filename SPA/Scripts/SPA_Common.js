function PaymentAjax(URLPATH,SuccessFunction)
{
    $.ajax({
        url: URLPATH,
        beforeSend: function () {
            if (!$(".loader").is(":visible"))
                $(".loader").show();
        },
        type: 'POST',
        async: true,
        success: function (result) {
            $("")
        },
        complete: function () {
            $(".loader").hide();
        },
        cache: false,
        contentType: false,
        processData: false
    });
}
