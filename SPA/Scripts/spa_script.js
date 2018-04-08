$(document).ready(function () {
    
    /*vacation block start*/
    $(".add-vac-dtl").click(function () {
        $(".add-vacation-block").slideToggle();
        //$(".search-box").hide();
    });
    $("#add-vac-dtl").click(function () {
        $(".add-vacation-block").slideToggle();
    });
    
    /*Calendar month tab collapse start*/
    //$(".month-name").click(function () {
    //    $(this).next().children().children().slideToggle(800);
    //    $(this).find('.cal-icon').toggleClass('fa-caret-up');
    //    $(this).parent().parent().siblings().find('.choose-your-product').slideUp();
    //});
    /*Calendar month tab collapse end*/

    /*end vacation block*/

    /*popover start*/
    $('[data-toggle="popover"]').popover({
        placement: 'bottom',
        trigger: 'hover'
    });
    /*end popover*/

    /*home page tab Start*/
    $('#myTab').tabCollapse();
    /*home page tab End*/
    /*menu start*/
    $(".right-btn a").click(function (event) {
        // lock scroll position, but retain settings for later
        var scrollPosition = [
            self.pageXOffset || document.documentElement.scrollLeft || document.body.scrollLeft,
            self.pageYOffset || document.documentElement.scrollTop || document.body.scrollTop
        ];
        event.preventDefault();
        if ($(this).hasClass("isdrag")) {
            $(".headertop-box").animate({
                right: "0px"
            }, 400);
            $(this).removeClass("isdrag").addClass("drag-toggle");
            $('.header-top-right').prepend('<div class="mask2"></div>');

            var html = jQuery('html');
            html.data('scroll-position', scrollPosition);
            html.data('previous-overflow', html.css('overflow'));
            html.css('overflow', 'hidden');
            window.scrollTo(scrollPosition[0], scrollPosition[1]);
        } else {
            $(".headertop-box").animate({
                right: "-270px"
            }, 400);
            $(this).addClass("isdrag").removeClass("drag-toggle");
            $('.mask2').remove();

            // un-lock scroll position
            var html = jQuery('html');
            var scrollPosition = html.data('scroll-position');
            html.css('overflow', html.data('previous-overflow'));
            window.scrollTo(scrollPosition[0], scrollPosition[1])
        }
        return false;
    });

    /*menu End*/
    /*Choose employee slider start*/
    var owl = $("#owl-demo");
    owl.owlCarousel({
        itemsCustom: [
          [0, 2],
          [450, 2],
          [600, 3],
          [700, 4],
          [1000, 4],
          [1200, 4]          
        ],
        navigation: true,
        autoPlay: true,
        navigationText: ["<i class='fa fa-angle-left icon-left'></i>", "<i class='fa fa-angle-right icon-right'></i>"]
    });
    /*Choose employee slider end*/

});
