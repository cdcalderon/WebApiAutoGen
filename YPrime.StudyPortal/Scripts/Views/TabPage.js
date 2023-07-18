var tabPageAttribute = "tabpage";
var activeClass = "active";

function TabPage() {
    this.init = function(tabPage) {
        //Set tab menu
        $("div.bhoechie-tab-menu>div.list-group>a").click(function(e) {
            e.preventDefault();
            if (!$(this).hasClass("disabled")) {
                $(this).siblings("a." + activeClass).removeClass(activeClass);
                $(this).addClass(activeClass);
                var index = $(this).index();
                $(".bhoechie-tab > .bhoechie-tab-content").removeClass(activeClass);
                $(".bhoechie-tab > .bhoechie-tab-content").eq(index).addClass(activeClass);
            }
        });
        //if a tab is specified highlight it
        if (tabPage !== null) {
            $("a[" + tabPageAttribute + '="' + tabPage + '"]').click();
        }
    };
}