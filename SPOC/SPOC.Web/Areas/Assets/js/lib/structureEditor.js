var StructureEditor = (function() {
    function init(id) {
        var self = this;
        var dataCache = { id: emptyGuid, title: "知识图谱" };
        var gooFlow;
        var gooFlowDataCache;
        var queryParam = { skip: 0, pageSize: 30 };
        var isCreate = guidIsEmpty(id);
        var saveActionToggle = true;//保存动作开关
        //初始化编辑器
        this.initEditor = function () {
            GooFlow.prototype.remarks.toolBtns = {
                cursor: "选择指针",
                direct: "结点连线",
                cube: "节点",
                tag: "知识点"
            };
            GooFlow.prototype.remarks.headBtns = {
                edit: "编辑名称",
                save: "保存"
            };

            GooFlow.prototype.remarks.extendRight = "工作区向右扩展";
            GooFlow.prototype.remarks.extendBottom = "工作区向下扩展";

            gooFlow = $.createGooFlow($("#editor"), {
                toolBtns: ["tag round", "cube"],
                headBtns: ["edit", "save"],
                initLabelText: dataCache.title,
                headLabel: true,
                haveTool: true,
                haveHead: true,
                haveDashed: false,
                haveGroup: false,
                useOperStack: true
            });

            $("#mapTitle").textbox("setValue", dataCache.title);

            gooFlow.onItemRightClick = function() {
                return false;
            };

            gooFlow.onItemDbClick = function(id, type) {
                var json = gooFlow.getItemInfo(id, type);
                if (json.type === "cube") {
                    return true;
                } else if (json.type === "tag round") {
                    showDialog();
                    gooFlowDataCache = { id: id, type: type, json: json };
                }
                return false;
            };

            gooFlow.onItemAdd = function(id, type, json) {
                if (json.type === "tag round" && stringIsEmpty(json.labelId)) {
                    gooFlowDataCache = { id: id, type: type, json: json };
                    showDialog();
                    return false;
                }
                return true;
            };

            gooFlow.onItemDel = function (id, type) {
                var json = gooFlow.getItemInfo(id, type);
                if (json != null && json.rootNode) {
                    return false;
                }
                return true;
            };

            gooFlow.onBtnSaveClick = function () {
                if (!saveActionToggle) {
                    return;
                }
                saveActionToggle = false;
                if (!checkLogin()) {
                    evtBus.dispatchEvt("show_login");
                    saveActionToggle = true;
                    return;
                }
                dataCache.mapData = JSON.stringify(gooFlow.exportData());
                
                var url = apiUrl + "StructureMap/";
                if (isCreate) {
                    url += "Create";
                } else {
                    url += "UpdateData";
                    dataCache.id = id;
                }
                VE.Mask("");
                nv.post(url,
                    dataCache,
                    function(data) {
                        VE.UnMask();
                        if (data.success) {
                            if (isCreate) {
                                dataCache = data.result;
                                id = dataCache.id;
                                isCreate = false;
                            }
                            $.messager.show({ title: "提示", msg: "保存成功！" });
                            evtBus.dispatchEvt("structure_map_change");
                        } else {
                            $.messager.alert("提示", data.error.message, "info");
                        }
                        saveActionToggle = true;
                    });
            };

            gooFlow.onBtnEditClick = function() {
                $("#titleDlg").dialog("open");
            };
            
            if (guidIsEmpty(id)) {
                var $editor = $("#editor");
                var l = $editor.width() / 2 - 100;
                var t = $editor.height() / 2 - 100;
                gooFlow.addNode(gooFlow.newId(),
                    {
                        "name": window.language,
                        "left": l,
                        "top": t,
                        "type": "cube",
                        "width": 100,
                        "height": 100,
                        "rootNode": true,
                        "alt": true
                    });
            } else {
                var url = apiUrl + "StructureMap/Get?id=" + id;
                VE.Mask("");
                nv.get(url, function (data) {
                    VE.UnMask();
                    if (data.success) {
                        dataCache = data.result;
                        $("#mapTitle").textbox("setValue", dataCache.title);
                        gooFlow.loadData(JSON.parse(data.result.mapData));
                    } else {
                        $.messager.alert("提示", data.error.message, "info");
                    }
                });
            }
        }

        //设置知识图谱名称
        this.changeTitle = function() {
            if (!$("#titleForm").form("validate")) {
                return;
            }
            dataCache.title = $("#mapTitle").textbox("getValue");
            gooFlow.setTitle(dataCache.title);
            $("#titleDlg").dialog("close");
        }

        //选择了标签
        this.selectLabel = function(index) {
            var row = $("#dg").datagrid("getRows")[index];
            $("#dg").datagrid("selectRow", index);
            gooFlowDataCache.json.labelId = row.id;
            var json = gooFlow.getItemInfo(gooFlowDataCache.id, gooFlowDataCache.type);
            if (json) {
                gooFlow.setName(gooFlowDataCache.id, row.title, gooFlowDataCache.type);
            } else {
                gooFlowDataCache.json.name = row.title;
                gooFlow.addNode(gooFlowDataCache.id, gooFlowDataCache.json);
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
            if (!checkLogin()) {
                evtBus.dispatchEvt("show_login");
                return;
            }
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