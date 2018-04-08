$(document).ready(function () {
    $("#Menu-page li a").click(function () {
        var Link = $(this).prop("name");
        var LangId = 0;
        $.each($("#LangChange li a"), function () {
            if ($(this).prop("class") == "activeShop") {
                if ($.trim($(this).html()) == "de") {
                    LangId = 2;
                }
                if ($.trim($(this).html()) == "fr") {
                    LangId = 3;
                }
                if ($.trim($(this).html()) == "en") {
                    LangId = 1;
                }
            }
        });
        if (Link.indexOf("#") >= 0) {
            var LinkPart = Link.split('#');
            if (LangId > 0)
                window.location.href = LinkPart[0] + "?lang=" + LangId + "#" + LinkPart[1];
            else
                window.location.href = LinkPart[0] + "#" + LinkPart[1];
        }
        else {
            if (LangId > 0)
                window.location.href = Link + "?lang=" + LangId;
            else
                window.location.href = Link;
        }
    });
    $("#ClickLogo").click(function () {
        var Link = $(this).prop("name");
        var LangId = 0;
        $.each($("#LangChange li a"), function () {
            if ($(this).prop("class") == "activeShop") {
                if ($.trim($(this).html()) == "de") {
                    LangId = 2;
                }
                if ($.trim($(this).html()) == "fr") {
                    LangId = 3;
                }
                if ($.trim($(this).html()) == "en") {
                    LangId = 1;
                }
            }
        });
        if (Link.indexOf("#") >= 0) {
            var LinkPart = Link.split('#');
            if (LangId > 0)
                window.location.href = LinkPart[0] + "?lang=" + LangId + "#" + LinkPart[1];
            else
                window.location.href = LinkPart[0] + "#" + LinkPart[1];
        }
        else {
            if (LangId > 0)
                window.location.href = Link + "?lang=" + LangId;
            else
                window.location.href = Link;
        }
    });

});

function remove_accent(str) {
    var a = array('À', 'Á', 'Â', 'Ã', 'Ä', 'Å', 'Æ', 'Ç', 'È', 'É', 'Ê', 'Ë', 'Ì', 'Í', 'Î', 'Ï', 'Ð', 'Ñ', 'Ò', 'Ó', 'Ô', 'Õ', 'Ö', 'Ø', 'Ù', 'Ú', 'Û', 'Ü', 'Ý', 'ß', 'à', 'á', 'â', 'ã', 'ä', 'å', 'æ', 'ç', 'è', 'é', 'ê', 'ë', 'ì', 'í', 'î', 'ï', 'ñ', 'ò', 'ó', 'ô', 'õ', 'ö', 'ø', 'ù', 'ú', 'û', 'ü', 'ý', 'ÿ', 'Ā', 'ā', 'Ă', 'ă', 'Ą', 'ą', 'Ć', 'ć', 'Ĉ', 'ĉ', 'Ċ', 'ċ', 'Č', 'č', 'Ď', 'ď', 'Đ', 'đ', 'Ē', 'ē', 'Ĕ', 'ĕ', 'Ė', 'ė', 'Ę', 'ę', 'Ě', 'ě', 'Ĝ', 'ĝ', 'Ğ', 'ğ', 'Ġ', 'ġ', 'Ģ', 'ģ', 'Ĥ', 'ĥ', 'Ħ', 'ħ', 'Ĩ', 'ĩ', 'Ī', 'ī', 'Ĭ', 'ĭ', 'Į', 'į', 'İ', 'ı', 'Ĳ', 'ĳ', 'Ĵ', 'ĵ', 'Ķ', 'ķ', 'Ĺ', 'ĺ', 'Ļ', 'ļ', 'Ľ', 'ľ', 'Ŀ', 'ŀ', 'Ł', 'ł', 'Ń', 'ń', 'Ņ', 'ņ', 'Ň', 'ň', 'ŉ', 'Ō', 'ō', 'Ŏ', 'ŏ', 'Ő', 'ő', 'Œ', 'œ', 'Ŕ', 'ŕ', 'Ŗ', 'ŗ', 'Ř', 'ř', 'Ś', 'ś', 'Ŝ', 'ŝ', 'Ş', 'ş', 'Š', 'š', 'Ţ', 'ţ', 'Ť', 'ť', 'Ŧ', 'ŧ', 'Ũ', 'ũ', 'Ū', 'ū', 'Ŭ', 'ŭ', 'Ů', 'ů', 'Ű', 'ű', 'Ų', 'ų', 'Ŵ', 'ŵ', 'Ŷ', 'ŷ', 'Ÿ', 'Ź', 'ź', 'Ż', 'ż', 'Ž', 'ž', 'ſ', 'ƒ', 'Ơ', 'ơ', 'Ư', 'ư', 'Ǎ', 'ǎ', 'Ǐ', 'ǐ', 'Ǒ', 'ǒ', 'Ǔ', 'ǔ', 'Ǖ', 'ǖ', 'Ǘ', 'ǘ', 'Ǚ', 'ǚ', 'Ǜ', 'ǜ', 'Ǻ', 'ǻ', 'Ǽ', 'ǽ', 'Ǿ', 'ǿ');
    var b = array('A', 'A', 'A', 'A', 'A', 'A', 'AE', 'C', 'E', 'E', 'E', 'E', 'I', 'I', 'I', 'I', 'D', 'N', 'O', 'O', 'O', 'O', 'O', 'O', 'U', 'U', 'U', 'U', 'Y', 's', 'a', 'a', 'a', 'a', 'a', 'a', 'ae', 'c', 'e', 'e', 'e', 'e', 'i', 'i', 'i', 'i', 'n', 'o', 'o', 'o', 'o', 'o', 'o', 'u', 'u', 'u', 'u', 'y', 'y', 'A', 'a', 'A', 'a', 'A', 'a', 'C', 'c', 'C', 'c', 'C', 'c', 'C', 'c', 'D', 'd', 'D', 'd', 'E', 'e', 'E', 'e', 'E', 'e', 'E', 'e', 'E', 'e', 'G', 'g', 'G', 'g', 'G', 'g', 'G', 'g', 'H', 'h', 'H', 'h', 'I', 'i', 'I', 'i', 'I', 'i', 'I', 'i', 'I', 'i', 'IJ', 'ij', 'J', 'j', 'K', 'k', 'L', 'l', 'L', 'l', 'L', 'l', 'L', 'l', 'l', 'l', 'N', 'n', 'N', 'n', 'N', 'n', 'n', 'O', 'o', 'O', 'o', 'O', 'o', 'OE', 'oe', 'R', 'r', 'R', 'r', 'R', 'r', 'S', 's', 'S', 's', 'S', 's', 'S', 's', 'T', 't', 'T', 't', 'T', 't', 'U', 'u', 'U', 'u', 'U', 'u', 'U', 'u', 'U', 'u', 'U', 'u', 'W', 'w', 'Y', 'y', 'Y', 'Z', 'z', 'Z', 'z', 'Z', 'z', 's', 'f', 'O', 'o', 'U', 'u', 'A', 'a', 'I', 'i', 'O', 'o', 'U', 'u', 'U', 'u', 'U', 'u', 'U', 'u', 'U', 'u', 'A', 'a', 'AE', 'ae', 'O', 'o');
    var i = 0;
    for (i; i <= a.length; i++) {
        if (str.indexOf(a[i])) {
            str = str.replaceAll(a[i], b[i]);
        }
    }
    if (i == a.length)
        return str;
}
function SetHidden(Id) {
    $("#" + Id + "_hidden").val($("#" + Id + " option:selected").text()).attr("valueId", $("#" + Id + " option:selected").val());
}
function GetCountry() {
    $.ajax({
        url: "http://lab.iamrohit.in/php_ajax_country_state_city_dropdown/api.php?type=getCountries",
        cache: true,
        success: function (Result) {
            console.log(Result);
        },
        error: function (request, status, error) { alert(status + ", " + error); }
    });
}
//function GetLocationData(Value, Effect, Who) {
//    debugger;
//    var Weburl = "";
//    if (Who == 1)
//        WebUrl = "http://lab.iamrohit.in/php_ajax_country_state_city_dropdown/api.php?type=getCountries";
//    if (Who == 2)
//        WebUrl = "http://lab.iamrohit.in/php_ajax_country_state_city_dropdown/api.php?type=getStates&countryId=" + Value;
//    if (Who == 3)
//        WebUrl = "http://lab.iamrohit.in/php_ajax_country_state_city_dropdown/api.php?type=getCities&stateId=" + Value;
//    if (Weburl != "") {
//        $.ajax({
//            url: "http://lab.iamrohit.in/php_ajax_country_state_city_dropdown/api.php?type=getCountries",
//            cache: true,
//            success: function (Result) {
//                console.log(Result); alert(Result);
//            },
//            error: function (request, status, error) { alert(status + ", " + error); }
//        });
//    }
//    else {

//        alert("hello");
//    }


//}


