var serviceUrl = "/api/services/app/";
var Record = (function () {
    function init() {
        var self = this;

        this.loadData = function () {
            $("#dg").datagrid("loading");
            var url = serviceUrl + "Upload/UserFiles";
            nv.get(url, function(data) {
                $("#dg").datagrid("loaded");
                if (data.success) {
                    $("#dg").datagrid("loadData", data.result);
                    var count = 0;
                    $.each(data.result, function(k, v) {
                        if (v.status < 2) {
                            count++;
                        }
                    });
                    evtBus.dispatchEvt("change_pending_file_count", count);
                } else {
                    $.messager.alert("提示", data.ERROR.Message, "info");
                }
            });
        };

        this.delete = function(id) {
            del(id);
        };

        this.deleteAll = function() {
            var rows = $("#dg").datagrid("getChecked");
            var ids = "";
            $.each(rows, function (k, v) {
                ids += v + ",";
            });
            ids = ids.substr(0, ids.length - 1);
            del(ids);
        };

        function del(idstr) {
            $("#dg").datagrid("loading");
            var url = serviceUrl + "Upload/Conceal?ids=" + idstr;
            nv.get(url, function (data) {
                $("#dg").datagrid("loaded");
                if (data.success) {
                    $.messager.show({ title: "提示", msg: "删除成功!" });
                    self.loadData();
                } else {
                    $.messager.alert("提示", data.ERROR.Message, "info");
                }
            });
        }
    }

    return init;
})();