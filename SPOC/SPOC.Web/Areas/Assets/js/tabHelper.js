var TabHelper = (function() {
    function init(containerId) {
        var self = this;
        var $tabContainer = parent.$("#" + containerId);
        var tabIndex;
        var htmlTemp = '<iframe name="{title}" scrolling="auto" frameborder="0"  src="{url}" style="width:100%;height:100%;line-height:0;display:block;margin:0;padding:0;"></iframe>';

        //开启新tab
        this.openTab = function(title, url, icon, titles) {
            var targetTitle;
            var hasTab = false;
            if (!$.isArray(titles)) {
                titles = [title];
            }

            $.each(titles, function(k, v) {
                if ($tabContainer.tabs("exists", v)) {
                    targetTitle = v;
                    hasTab = true;
                    return false;
                }
                return true;
            });
            if (hasTab) {
                $tabContainer.tabs("select", targetTitle);
                var tab = $tabContainer.tabs("getSelected");
                $tabContainer.tabs("update", {
                    tab: tab,
                    options: {
                        title: title,
                        content: htmlTemp.format({ title: title, url: url }),
                        icon: icon
                    }
                });
            } else {
                $tabContainer.tabs("add", {
                    title: title,
                    content: htmlTemp.format({ title: title, url: url }),
                    closable: true,
                    icon: icon
                });
            }
        };

        //获取tabs控件当前selected状态tab的index
        this.getTabIndex = function () {
            var tab = $tabContainer.tabs("getSelected");
            tabIndex = $tabContainer.tabs("getTabIndex", tab);
            return tabIndex;
        };

        //关闭当前tab
        this.closeTab = function (index) {
            if (index) {
                $tabContainer.tabs("close", index);
            } else {
                if (tabIndex) {
                    $tabContainer.tabs("close", tabIndex);
                } else {
                    $tabContainer.tabs("close", self.getTabIndex());
                }
            }
        };

        //选中index指定的tab
        this.select = function(tabIndex) {
            $tabContainer.tabs("select", tabIndex);
        };

        self.getTabIndex();
    }

    return init;
})();