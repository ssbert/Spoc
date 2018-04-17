var StructureEditor = (function() {
    function init() {
        var self = this;
        var gooFlow;
        var dataCache;
        var queryParam = { skip: 0, pageSize: 30 };
        //初始化编辑器
        this.initEditor = function () {
            GooFlow.prototype.remarks.toolBtns = {
                cursor: "选择指针",
                direct: "结点连线",
                cube: "节点",
                tag: "标签"
            };
            GooFlow.prototype.remarks.headBtns = {
                save: "保存"
            };

            GooFlow.prototype.remarks.extendRight = "工作区向右扩展";
            GooFlow.prototype.remarks.extendBottom = "工作区向下扩展";

            gooFlow = $.createGooFlow($("#editor"), {
                toolBtns: ["tag round", "cube"],
                headBtns: ["save"],
                initLabelText: "知识图谱编辑器",
                headLabel: true,
                haveTool: true,
                haveHead: true,
                haveDashed: false,
                haveGroup: false,
                useOperStack: true
            });

            gooFlow.onItemRightClick = function() {
                return false;
            };

            gooFlow.onItemDbClick = function(id, type) {
                var json = gooFlow.getItemInfo(id, type);
                if (json.type === "cube") {
                    return true;
                } else if (json.type === "tag round") {
                    showDialog();
                    dataCache = { id: id, type: type, json: json };
                }
                return false;
            };

            gooFlow.onItemAdd = function(id, type, json) {
                if (json.type === "tag round" && stringIsEmpty(json.labelId)) {
                    dataCache = { id: id, type: type, json: json };
                    showDialog();
                    return false;
                }
                return true;
            };

            gooFlow.onBtnSaveClick = function() {
                var data = JSON.stringify(gooFlow.exportData());
                $.post("/Lib/Manage/SaveData",
                    { data: data },
                    function(result) {
                        if (result.success) {
                            $.messager.show({ title: "提示", msg: "保存成功!" });
                        } else {
                            $.messager.alert("提示", data.error, "info");
                        }
                    });
            };

            gooFlow.onItemDel = function(id, type) {
                var json = gooFlow.getItemInfo(id, type);
                if (json != null && json.rootNode) {
                    return false;
                }
                return true;
            };

            $.ajax({
                url: "/files/StructureData/structureData.json?v=" + Date.parse(new Date()),
                dataType: "json",
                type: "get",
                success: function (data, textStatus, jqxhr) {
                    gooFlow.loadData(data);
                },
                error: function (jqxhr, textStatus, errorThrown) {
                    var $editor = $("#editor");
                    var l = $editor.width() / 2 - 100;
                    var t = $editor.height() / 2 - 100;
                    gooFlow.addNode(gooFlow.newId(), {
                        "name": window.language,
                        "left": l,
                        "top": t,
                        "type": "cube",
                        "width": 100,
                        "height": 100,
                        "rootNode" :true,
                        "alt": true});
                }
            });
        }

        //选择了标签
        this.selectLabel = function(index) {
            var row = $("#dg").datagrid("getRows")[index];
            $("#dg").datagrid("selectRow", index);
            dataCache.json.labelId = row.id;
            var json = gooFlow.getItemInfo(dataCache.id, dataCache.type);
            if (json) {
                gooFlow.setName(dataCache.id, row.title, dataCache.type);
            } else {
                dataCache.json.name = row.title;
                gooFlow.addNode(dataCache.id, dataCache.json);
            }
            $("#tagDlg").dialog("close");
        };

        //初始化标签列表
        this.initDataGrid = function() {
            $("#dg").datagrid({
                    onSortColumn: function(sort, order) {
                        queryParam.sort = sort;
                        queryParam.order = order;
                        loadData(queryParam);
                    }
                })
                .datagrid("getPager")
                .pagination({
                    onSelectPage: function(pageNumber, pageSize) {
                        queryParam.pageNumber = pageNumber;
                        queryParam.skip = (pageNumber - 1) * pageSize;
                        if (queryParam.skip < 0) {
                            queryParam.skip = 0;
                        }
                        queryParam.pageSize = pageSize;
                        loadData(queryParam);
                    },
                    onChangePageSize: function(pageSize) {
                        queryParam.pageSize = pageSize;
                        loadData(queryParam);
                    }
                });
        };

        //查询标签数据
        this.query = function() {
            getQueryParam();
            queryParam.skip = 0;
            queryParam.exceptList = getExceptIdList();
            loadData(queryParam);
        };

        //获取地图上已有的标签Id
        function getExceptIdList() {
            var idList = [];
            var data = gooFlow.exportData();
            $.each(data.nodes, function(k, node) {
                if (node.type === "tag round") {
                    idList.push(node.labelId);
                }
            });
            return idList;
        }

        function loadData(param) {
            $("#dg").datagrid("loading");
            var url = apiUrl + "liblabel/GetPagination";
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
            queryParam.code = $("#code").textbox("getValue").trim();
            queryParam.title = $("#title").textbox("getValue").trim();
            queryParam.userLoginName = $("#userLoginName").textbox("getValue").trim();
            queryParam.userFullName = $("#userFullName").textbox("getValue").trim();
        }

        function showDialog() {
            $("#tagDlg").dialog("open");
            self.query();
        }
    }

    
    return init;
})();