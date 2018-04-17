var Notification = (function() {
    function init() {
        var attr = {};

        this.setInitData = function (handle, typeCode, content, classIds) {
            attr.handle = handle;
            attr.typeCode = typeCode;
            attr.content = content;
            attr.classIds = classIds;
            loadData();
        };
        
        this.send = function() {
            var url = apiUrl + "Notification/Send";
            var param = {
                content: $("#content").textbox("getValue"),
                typeCode: attr.typeCode,
                classIds: attr.classIds
            };
            VE.Mask("");
            nv.post(url,
                param,
                function(data) {
                    VE.UnMask();
                    if (data.success) {
                        $.messager.alert("提示",
                            "发送成功，即将关闭页面！",
                            "info",
                            function() {
                                evtBus.dispatchEvt("notification_finished", {handle: attr.handle});
                            });
                    } else {
                        $.messager.alert("提示", data.error.message, "info");
                    }
                });
        };

        function loadData () {
            var url = apiUrl + "Department/GetClassesByIdList";
            $("#dg").datagrid("loading");
            nv.post(url,
                { idList: attr.classIds },
                function (data) {
                    $("#dg").datagrid("loaded");
                    if (data.success) {
                        $("#dg").datagrid("loadData", data.result);
                    }
                });
            $("#content").textbox("setValue", attr.content);
        }

    }

    return init;
})();