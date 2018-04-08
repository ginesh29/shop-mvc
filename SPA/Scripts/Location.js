function ajaxCall() {
    this.send = function (data, url, method, success, type) {
        type = type || 'json';
        var successRes = function (data) {
            success(data);
        }

        var errorRes = function (e) {
            console.log(e);
            alert("Error found \nError Code: " + e.status + " \nError Message: " + e.statusText);
        }
        $.ajax({
            url: url,
            type: method,
            data: data,
            success: successRes,
            error: errorRes,
            dataType: type,
            timeout: 60000
        });

    }

}

function locationInfo() {
    var rootUrl = "/Clickandgo/GetLocation";
    var call = new ajaxCall();
    this.getCities = function (id, effect, current) {
        $("#" + effect + " option:gt(0)").remove();
        var url = rootUrl + '?Location=2&Id=' + id;
        var method = "post";
        var data = {};
        $("#" + effect).find("option:eq(0)").html("Please wait..");
        call.send(data, url, method, function (data) {
            $("#" + effect).html("<option value>select city</option>");
            $.each(data, function (key, value) {
                var option = $('<option />');
                option.attr('value', data[key].LocId).text(data[key].name);
                $("#" + effect).append(option);
                if ((data.length - 1) == key)
                {
                    if ($("#" + effect + "_hidden").val() != "" && $("#" + effect + "_hidden").attr("valueId") != "") {
                        $("#" + effect + " option[value='" + $("#" + effect + "_hidden").attr("valueId") + "']").attr("selected", true).trigger('change');
                        $("#" + effect + "_hidden").attr("valueId", "");
                    }
                    else
                        SetHidden(current, effect);
                }
            });
            $("#" + effect).prop("disabled", false);

        });
    };
    this.getStates = function (id, effect, current) {
        $("#" + effect + " option:gt(0)").remove();
        $("#City option:gt(0)").remove();
        var url = rootUrl + '?Location=1&Id=' + id;
        var method = "post";
        var data = {};
        $("#" + effect).find("option:eq(0)").html("Please wait..");
        call.send(data, url, method, function (data) {
            $("#" + effect).html("<option value>select state</option>");
            $.each(data, function (key, value) {
                var option = $('<option />');
                option.attr('value', data[key].LocId).text(data[key].name);
                $("#" + effect).append(option);
                if ((data.length - 1) == key)
                {
                    if ($("#"+effect+"_hidden").val() != "" && $("#"+effect+"_hidden").attr("valueId") != "") {
                        $("#"+effect+" option[value='" + $("#" + effect + "_hidden").attr("valueId") + "']").attr("selected", true).trigger('change');
                        $("#" + effect + "_hidden").attr("valueId", "");
                    }
                    else
                        SetHidden(current, effect);
                }
                    
            });
            $("#" + effect).prop("disabled", false);
        });
    };
    //this.getCountries = function (id, effect) {
    //    var url = rootUrl + '?type=getCountries';
    //    var method = "post";
    //    var data = {};
    //    $('.countries').find("option:eq(0)").html("Please wait..");
    //    call.send(data, url, method, function (data) {
    //        $('.countries').find("option:eq(0)").html("Select Country");
    //        console.log(data);
    //        if (data.tp == 1) {
    //            $.each(data['result'], function (key, val) {
    //                var option = $('<option />');
    //                option.attr('value', key).text(val);
    //                $('.countries').append(option);
    //            });
    //            $(".countries").prop("disabled", false);
    //        }
    //        else {
    //            alert(data.msg);
    //        }
    //    });
    //};
}
function SetCurrentlocation(city, country, state) {
    $.ajax({
        url: "http://freegeoip.net/json/",
        type: 'GET',
        dataType: "json",
        async: true,
        success: function (response) {
            var loc = new locationInfo();
            loc.getStates($("option[html=" + country + ":selected").val(), state);
            if ($("input[name='City_hidden']").val() == "")
                $("input[name='City_hidden']").val(response.city);
            if ($("input[name='Country_hidden']").val() == "")
                $("input[name='Country_hidden']").val(response.country_name);
            if ($("input[name='State_hidden']").val() == "")
                $("input[name='State_hidden']").val(response.region_name);
            if ($("input[name='Zipcode']").val() == "")
                $("input[name='Zipcode']").val(response.zip_code);
        },
        error: function (request, status, error) { alert(status + ", " + error); }
    });
}

$(function () {
    var loc = new locationInfo();
    //loc.getCountries();

});

function GetLocationData(Value, Effect, Who, current) {
    var loc = new locationInfo();
    var Weburl = "";

    if (Who == 1) {
        WebUrl = "http://lab.iamrohit.in/php_ajax_country_state_city_dropdown/api.php?type=getCountries";
        loc.getCountries();
        SetHidden(Effect, current);
    }
    if (Who == 2) {
        WebUrl = "http://lab.iamrohit.in/php_ajax_country_state_city_dropdown/api.php?type=getStates&countryId=" + Value;
        loc.getStates(Value, Effect, current);
    }
    if (Who == 3) {

        WebUrl = "http://lab.iamrohit.in/php_ajax_country_state_city_dropdown/api.php?type=getCities&stateId=" + Value;
        loc.getCities(Value, Effect, current);
    }
}
function SetHidden(Effect, Data) {
    $("#" + Effect + "_hidden").val($("#" + Effect + " option:selected").text());
    if (Data != "")
        $("#" + Data + "_hidden").val("");
}

