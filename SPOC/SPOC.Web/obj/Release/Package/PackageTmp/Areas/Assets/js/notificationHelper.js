//注意：一个实例对应一个通知控件，重复调用open会覆盖之前开启的通知界面
var NotificationHelper = (function () {
    function init(tabId) {
        var attr = {};
        var handle = new Date().getTime();
        var tab = new TabHelper(tabId);
        var tabIndex;
        var notification;
        
        var evtHandle = evtBus.addEvt("notification_init", function(data) {
            notification = data;
            notification.setInitData(handle, attr.typeCode, attr.content, attr.classIds);
        });

        var evtHandle2 = evtBus.addEvt("notification_finished", function (data) {
            if (data.handle !== handle) {
                return;
            }
            if (attr.callback) {
                attr.callback();
            }
            tab.closeTab(tabIndex);
            tabIndex = null;
        });

        $(window).unload(function () {
            evtBus.removeEvt(evtHandle);
            evtBus.removeEvt(evtHandle2);
        });
        
        //开启通知界面
        this.open = function(typeCode, classIds, content, callback) {
            if (!$.isArray(classIds) || classIds.length <= 0) {
                return;
            }
            if (tabIndex) {
                tab.closeTab(tabIndex);
            }
            attr.typeCode = typeCode;
            attr.classIds = classIds;
            attr.content = content;
            attr.callback = callback;
            var url = "/User/Notification";
            tab.openTab("发送通知", url, "icon-email_go");
            tabIndex = tab.getTabIndex();
        };
    }

    return init;
})();