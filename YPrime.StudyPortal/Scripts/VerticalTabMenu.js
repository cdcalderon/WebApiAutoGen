var YPrime = YPrime || {};

YPrime.VerticalTabMenu = (function() {
    "use strict";
    var _disabledClass = "disabled";
    var _activeClass = "active";
    var _loadedClass = "vertical-menu-content-loaded";
    var _lazyLoadConfigs = {};

    return {
        LoadContent: function(config) {
            var contentContainer = $(config.ContentContainerId);
            contentContainer.load(config.ContentUrl,
                function() {
                    $(config.MenuItemId).addClass(_loadedClass); //mark container as loaded
                    if (config.Callback != undefined) { //execute callback
                        window[config.Callback]();
                    }
                });

        },

        Initialize: function(configs) { //array of configurations
            var self = this;

            $("div.bhoechie-tab-menu>div.list-group>a").click(function(e) {
                e.preventDefault();
                if (!$(this).hasClass(_disabledClass)) {
                    $(this).siblings("a." + _activeClass).removeClass(_activeClass);
                    $(this).addClass(_activeClass);
                    if (!$(this).hasClass(_loadedClass)) { //if content isn't already loaded, then load
                        var configToLoad = _lazyLoadConfigs["#" + $(this).attr("id")];
                        if (configToLoad != undefined) {
                            self.LoadContent(configToLoad);
                        }
                    }
                    var index = $(this).index();
                    $(".bhoechie-tab > .bhoechie-tab-content").removeClass(_activeClass);
                    $(".bhoechie-tab > .bhoechie-tab-content").eq(index).addClass(_activeClass);
                }
            });

            //eager loaded content
            for (var i = 0; i < configs.length; i++) {
                if (configs[i].LazyLoad == true) {
                    _lazyLoadConfigs[configs[i].MenuItemId] = configs[i];
                } else {
                    LoadContent(configs[i]);
                }
            }

            var firstMenuItem = $("div.bhoechie-tab-menu>div.list-group>a:first");
            if (!firstMenuItem.hasClass(self._loadedClass)) {
                var config = _lazyLoadConfigs[firstMenuItem.attr("id")];
                if (config != undefined) {
                    self.LoadContent(_lazyLoadConfigs[firstMenuItem.attr("id")]);
                }
            }
        }
    };
}());