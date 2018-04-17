(function(){

	var obj = $('body'),
        nav = $('nav'),
        footer = $('.footer'),
        container = $('.container-box');

    setFooter();

    $(window).on("resize", function () {
        setTimeout(function () {
            setFooter();
        }, 200);
    })

    function setFooter() {
        var winHeight = $(window).outerHeight(),
            bodyHeight = obj.outerHeight(),
            navHeight = nav.outerHeight() || 0,
            footerHeight = footer.outerHeight() || 0;

        // if (bodyHeight < winHeight) {
        container.css("minHeight", winHeight - (navHeight + footerHeight) + "px");
        // };
    }

    // 阻止点击下拉窗口时，窗口关闭
    $(".dropdown-menu").on('click', function (e) {
        e.stopPropagation();
    });

})()