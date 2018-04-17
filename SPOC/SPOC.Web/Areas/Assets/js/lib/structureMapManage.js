var StructureMapManage = (function() {
    function init() {
        var self = this;
        var tabHelper = new TabHelper("tabs");
        var tabIndex = tabHelper.getTabIndex();
        var hasChange = false;

        var evtHandle = evtBus.addEvt("structure_map_change", function() {
            hasChange = true;
        });
        var evtHandle2 = evtBus.addEvt("tabs_tab_change", function (data) {
            if (!hasChange || data.index !== tabIndex) {
                return;
            }
            hasChange = false;
            self.query();
        });
        $(window).unload(function () {
            evtBus.removeEvt(evtHandle);
            evtBus.removeEvt(evtHandle2);
        });

        this.showEditor = function(id) {
            if (!checkLogin()) {
                evtBus.dispatchEvt("show_login");
                return;
            }
            var url = "/Lib/Manage/StructureEditor";
            var title = "新增知识图谱";
            if (!guidIsEmpty(id)) {
                url += "?id=" + id;
                title = "编辑知识图谱";
            }
            tabHelper.openTab(title, url, "icon-add", ["新增知识图谱", "编辑知识图谱"]);
        };

        //删除
        this.del = function (index) {
            var ids = [];
            if (index) {
                var row = $("#dg").datagrid("getRows")[index];
                $("#dg").datagrid("selectRow", index);
                ids.push(row.id);
            } else {
                var rows = $("#dg").datagrid("getChecked");
                $.each(rows, function(k, v) { ids.push(v.id); });
            }
            if (ids.length === 0) {
                $.messager.alert("提示", "请选择要删除的项", "info");
                return;
            }

            $.messager.confirm("删除确认",
                "确认进行删除操作吗？",
                function(b) {
                    if (!b) {
                        return;
                    }
                    if (!checkLogin()) {
                        evtBus.dispatchEvt("show_login");
                        return;
                    }

                    var url = apiUrl + "StructureMap/Delete";
                    $("#dg").datagrid("loading");
                    nv.post(url,
                        { idList: ids },
                        function(data) {
                            $("#dg").datagrid("loaded");
                            if (data.success) {
                                $.messager.show({ title: "提示", msg: "删除成功！" });
                                self.query();
                            } else {
                                $.messager.alert("提示", data.error.message, "info");
                            }
                        });
                });
        };

        //设置主图
        this.setMain = function(index) {
            if (!checkLogin()) {
                evtBus.dispatchEvt("show_login");
                return;
            }
            var row = $("#dg").datagrid("getRows")[index];
            $("#dg").datagrid("loading");
            var url = apiUrl + "StructureMap/SetIsMain?id=" + row.id;
            nv.get(url,
                function (data) {
                    $("#dg").datagrid("loaded");
                    if (data.success) {
                        $.messager.show({ title: "提示", msg: "操作成功！" });
                        self.query();
                    } else {
                        $.messager.alert("提示", data.error.message, "info");
                    }
                });
        };

        //显示
        this.show = function (index) {
            updateIsShow(index, true);
        };

        //隐藏
        this.hide = function(index) {
            updateIsShow(index, false);

        };

        //查询数据
        this.query = function () {
            if (!checkLogin()) {
                evtBus.dispatchEvt("show_login");
                return;
            }
            $("#dg").datagrid("loading");
            var url = apiUrl + "StructureMap/GetList?ts=" + Date.parse(new Date());
            nv.get(url,
                function (data) {
                    $("#dg").datagrid("loaded");
                    if (data.success) {
                        $("#dg").datagrid("loadData", data.result);
                    } else {
                        $.messager.alert("提示", data.error.message, "info");
                    }
                });
        };

        //更新设置是否显示
        function updateIsShow(index, isShow) {
            if (!checkLogin()) {
                evtBus.dispatchEvt("show_login");
                return;
            }
            var row = $("#dg").datagrid("getRows")[index];
            $("#dg").datagrid("loading");
            var url = apiUrl + "StructureMap/UpdateIsShow";
            nv.post(url,
                { key: row.id, value: isShow },
                function(data) {
                    $("#dg").datagrid("loaded");
                    if (data.success) {
                        $.messager.show({ title: "提示", msg: "操作成功！" });
                        self.query();
                    } else {
                        $.messager.alert("提示", data.error.message, "info");
                    }
                });
        }
    }

    return init;
})();