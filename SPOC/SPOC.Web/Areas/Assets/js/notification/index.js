﻿/**
 * 知识库标签管理
 */
var serviceUrl = "/api/services/app/";
var iframeHtml = '<iframe name="{title}" scrolling="auto" frameborder="0"  src="{url}" style="width:100%;height:100%;line-height:0;display:block;margin:0;padding:0;"></iframe>';
var LibListView = (function () {
    function init() {
        var self = this;
        var paramCache = { //缓存查询数据
            code: "",
            title: "",
            skip: 0,
            pageSize: 30
        };
        var isListChanged = false;//记录列表是否有刷新动作
        var tab = parent.$("#tabs").tabs("getSelected");
        var selfTabIndex = parent.$("#tabs").tabs("getTabIndex", tab);
        var category = new nv.category.CombotreeDataClass("folderId", "lib_lable");
        var handle = evtBus.addEvt("update_liblable_list", function () {
            isListChanged = true;
            getData(paramCache);
        });

        var handle2 = evtBus.addEvt("tabs_tab_change", function(data) {
            if (!isListChanged || data.index !== selfTabIndex) {
                return;
            }
            isListChanged = false;
            $("#dg").datagrid("fixRowHeight");
        });

        $(window).unload(function () {
            evtBus.removeEvt(handle);
            evtBus.removeEvt(handle2);
        });

        this.query = function () {
            var param = getQueryParam();
            param.skip = 0;
            getData(param);
        };

        this.create = function () { 
            var url = "/lib/Manage/Edit";
            var title = "新增知识标签";

            parent.$("#tabs")
                .tabs("add",
                    {
                        title: title,
                        content: iframeHtml.format({ title: title, url: url }),
                        closable: true,
                        icon: "icon-add"
                    });
        };

        this.edit = function (index) {
            $("#dg").datagrid("selectRow", index);
            var row = $("#dg").datagrid("getRows")[index];

            var url = "/lib/Manage/Edit?id={0}";
            var title = "编辑知识标签";
            openTab(title, url.format(row.id, row.paperTypeCode), "icon-edit"); 
        };

        this.del = function () {
            var checkedRows = $("#dg").datagrid("getChecked");
            if (checkedRows.length === 0) {
                $.messager.alert("提示", "请先选择要删除的知识标签", "info");
                return;
            }

            $.messager.confirm("删除确认",
                "确认进行删除操作吗？",
                function (b) {
                    if (!b) {
                        return;
                    }
                    if (!checkLogin()) {
                        evtBus.dispatchEvt("show_login");
                        return;
                    }
                    var idArray = [];
                    $.each(checkedRows,
                        function (k, v) {
                            idArray.push(v.id);
                        });
                    var url = serviceUrl + "liblable/delete?ids=" + idArray.join(",");
                    VE.Mask("");
                    nv.get(url, function (data) {
                        VE.UnMask();
                        if (data.success) {
                            $.messager.show({ title: "提示", msg: "删除成功！" });
                            getData(paramCache);
                        } else {
                            $.messager.alert("提示", data.error.message, "info");
                        }
                    });

                });
        }

  
    

        this.flush = function () {
            getData(paramCache);
        };

        function openTab(title, url, icon) {
            if (parent.$("#tabs").tabs("exists", title)) {
                parent.$("#tabs").tabs("select", title);
                var tab = parent.$("#tabs").tabs("getSelected");
                parent.$("#tabs").tabs("update", {
                    tab: tab,
                    options: {
                        title: title,
                        content: iframeHtml.format({ title: title, url: url }),
                        icon: icon
                    }
                });
            } else {
                parent.$("#tabs").tabs("add", {
                    title: title,
                    content: iframeHtml.format({ title: title, url: url }),
                    closable: true,
                    icon: icon
                });
            }
        }

        function initPagination() {
            $("#dg")
                .datagrid("getPager")
                .pagination({
                    onSelectPage: function (pageNumber, pageSize) {
                        paramCache.pageNumber = pageNumber;
                        paramCache.skip = (pageNumber - 1) * pageSize;
                        if (paramCache.skip < 0) {
                            paramCache.skip = 0;
                        }
                        paramCache.pageSize = pageSize;
                        getData(paramCache);
                    },
                    onChangePageSize: function (pageSize) {
                        paramCache.pageSize = pageSize;
                        getData(paramCache);
                    }
                });
        }

        function getData(param) {
            $("#dg").datagrid("loading");
            var url = serviceUrl + "liblable/GetPagination";
            nv.post(url,
                param,
                function (data) {
                    $("#dg").datagrid("loaded");
                    if (data.success) {
                        $("#dg")
                            .datagrid("loadData", data.result.rows)
                            .datagrid("getPager")
                            .pagination({
                                pageNumber: param.pageNumber,
                                total: data.result.total
                            });
                    } else {
                        $.messager.alert("提示", data.error.message, "info");
                    }
                });
        }

        function getQueryParam() {
            paramCache.title = $("#title").textbox("getValue").trim();
            paramCache.userLoginName = $("#userLoginName").textbox("getValue").trim();
            paramCache.userFullName = $("#userFullName").textbox("getValue").trim();
            paramCache.folderId = $("#folderId").combotree("getValues");
            return paramCache;
        }

        $(function () {
            category.getCategory();
            initPagination();
            self.query();
        });
    }
    return init;
})();