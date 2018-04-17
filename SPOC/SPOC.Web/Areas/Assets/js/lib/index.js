/**
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
       
        var category = new nv.category.CombotreeDataClass("folderId", "lib_label");
        var handle = evtBus.addEvt("update_liblabel_list", function () {
           getData(paramCache);
           // self.query();
           // $('#dg').datagrid('reload');
        });
    

        $(window).unload(function () {
            evtBus.removeEvt(handle);
           
        });
        $(window).unload(function () {
            evtBus.removeEvt(handle);

        });

        this.query = function () {
            var param = getQueryParam();
            param.skip = 0;
            getData(param);
        };

        this.create = function () { 
            var url = "/lib/Manage/EditLabel";
            var title = "新增知识点";
            if (!checkLogin()) {
                evtBus.dispatchEvt("show_login");
                return;
            }
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
            if (!checkLogin()) {
                evtBus.dispatchEvt("show_login");
                return;
            }
            $("#dg").datagrid("selectRow", index);
            var row = $("#dg").datagrid("getRows")[index];

            var url = "/lib/Manage/EditLabel?id={0}";
            var title = "编辑知识点";
            openTab(title, url.format(row.id, row.paperTypeCode), "icon-edit"); 
        };

        this.del = function () {
         
            var checkedRows = $("#dg").datagrid("getChecked");
            if (checkedRows.length === 0) {
                $.messager.alert("提示", "请先选择要删除的知识点", "info");
                return;
            }
            if (!checkLogin()) {
                evtBus.dispatchEvt("show_login");
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
                    var url = serviceUrl + "liblabel/delete?ids=" + idArray.join(",");
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
            if (!checkLogin()) {
                evtBus.dispatchEvt("show_login");
                return;
            }
            $("#dg").datagrid("loading");
            var url = serviceUrl + "liblabel/GetPagination";
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