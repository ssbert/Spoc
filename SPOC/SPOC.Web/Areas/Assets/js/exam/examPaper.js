var serviceUrl = "/api/services/app/";
var iframeHtml = '<iframe name="{title}" scrolling="auto" frameborder="0"  src="{url}" style="width:100%;height:100%;line-height:0;display:block;margin:0;padding:0;"></iframe>';
var dgFormatter = {
    outdatedDate: function (value, row, index) {
        if (value === 0) {
            return "不限";
        }
        return new Date(value * 1000).format("yyyy-MM-dd hh:mm:ss");
    },
    questionStatusCode: function (value, row, index) {
        switch (value) {
            case "normal":
                return "正常";
            case "outdated":
                return "已过期";
            case "disabled":
                return "禁用";
            case "draft":
                return "草稿";
            default:
                return "正常";
        }
    },
    questionText: function (value, row, index) {
        value = value.replace(/<[^>]+>/g, "");
        if (value.length > 60) {
            value = value.substr(0, 60) + "...";
        }
        return value;
    }
}
//试卷管理界面
var PaperListViewClass = (function () {
    function init() {
        var self = this;
        var paramCache = { //缓存查询数据
            paperCode: "",
            paperName: "",
            paperTypeCode: "",
            departmentUidList: [],
            subjectUidList: [],
            folderUidList:[],
            skip: 0,
            pageSize: 30
        };
        var isListChanged = false;//记录列表是否有刷新动作
        var tab = parent.$("#tabs").tabs("getSelected");
        var selfTabIndex = parent.$("#tabs").tabs("getTabIndex", tab);
        var category = new nv.category.CombotreeDataClass("folderUid", "exam_paper");

        var handle = evtBus.addEvt("update_paper_list", function () {
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

        this.create = function (paperTypeCode) {
            if (!checkLogin()) {
                evtBus.dispatchEvt("show_login");
                return;
            }
            var url = "/ExamPaper/Manage/Edit?paperTypeCode=" + paperTypeCode;
            var title = "创建固定试卷";
            if (paperTypeCode === "random") {
                title = "创建随机试卷";
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

            var url = "/ExamPaper/Manage/Edit?id={0}&paperTypeCode={1}";
            var title = "编辑试卷";
            openTab(title, url.format(row.id, row.paperTypeCode), "icon-edit"); 
        };

        this.del = function () {
            var checkedRows = $("#dg").datagrid("getChecked");
            if (checkedRows.length === 0) {
                $.messager.alert("提示", "请先选择要删除的试卷", "info");
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
                    var url = serviceUrl + "ExamPaper/Delete?ids=" + idArray.join(",");
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

        this.preview = function (index) {
            if (!checkLogin()) {
                evtBus.dispatchEvt("show_login");
                return;
            }
            $("#dg").datagrid("selectRow", index);
            var row = $("#dg").datagrid("getRows")[index];

            var url = "/ExamPaper/Manage/Preview?";
            if (row.paperTypeCode === "fix") {
                url += "paperUid=" + row.id;
            } else if (row.paperTypeCode === "random") {
                url += "policyUid=" + row.id;
            }
            var title = "预览试卷";

            openTab(title, url, "icon-page_magnify"); 
        };

        this.exportToWord = function (index) {
            if (!checkLogin()) {
                evtBus.dispatchEvt("show_login");
                return;
            }
            $("#dg").datagrid("selectRow", index);
            var row = $("#dg").datagrid("getRows")[index];
            var url = "/ExamPaper/Manage/ExportToWord/" + row.id;
            window.open(url);
        }

        this.flush = function () {
            getData(paramCache);
        };

        this.showImportWindow = function () {
            if (!checkLogin()) {
                evtBus.dispatchEvt("show_login");
                return;
            }
            $("#import-form").form("reset");
            $("#wd").window("open");
        }

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
            var url = serviceUrl + "ExamPaper/GetPagination";
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
            paramCache.paperCode = $("#paperCode").textbox("getValue").trim();
            paramCache.paperName = $("#paperName").textbox("getValue").trim();
            paramCache.paperTypeCode = $("#paperTypeCode").combobox("getValue");
            paramCache.folderUidList = $("#folderUid").combotree("getValues");
            paramCache.userLoginName = $("#userLoginName").textbox("getValue").trim();
            paramCache.userFullName = $("#userFullName").textbox("getValue").trim();
            return paramCache;
        }

        $(function() {
            category.getCategory();
            initPagination();
            self.query();
        });
    }
    return init;
})();

var paperEdit = {
//试卷基础信息
    PaperBaseInfoClass: (function () {

        function init(id, paperNode) {
            var examPaper = { paperTypeCode: "fix" };
            var category = new nv.category.CombotreeDataClass("folderUid", "exam_paper");
            var handle = evtBus.addEvt("total_score_change", onTotalScoreChange);
            $(window).unload(function () {
                evtBus.removeEvt(handle);
            });

            this.save = function () {
                var validate = $("#baseInfo-form").form("validate");
                if (!validate) {
                    return;
                }
                if (!checkLogin()) {
                    evtBus.dispatchEvt("show_login");
                    return;
                }
                var param = getParam();
                var url = serviceUrl + "ExamPaper/";
                var isCreate = stringIsEmpty(param.id);
                if (isCreate) {
                    url += "Create";
                } else {
                    url += "Update";
                }
                VE.Mask("");
                nv.post(url, param, function (data) {
                    VE.UnMask();
                    if (data.success) {
                        if (isCreate) {
                            setData(data.result);
                            paperNode.show(examPaper.id);
                        }
                        evtBus.dispatchEvt("update_paper_list");
                        $.messager.show({ title: "提示", msg: "保存成功!" });
                    } else {
                        $.messager.alert("提示", data.error.message, "info");
                    }
                });
                    
            };

            this.getData = function () {
                //返回一个examPaper对象的副本，避免examPaper对象在外部被修改
                return $.extend({}, examPaper);
            };

            this.setTotalScort = function (value) {
                $("#totalScore").text(value);
            };

            this.onChangeCodeMode = function (checked) {
                var isCustomCode = !checked;
                $("#paperCode").textbox("readonly", !isCustomCode)
                    .textbox({ required: isCustomCode });
                if (!isCustomCode) {
                    $("#paperCode").textbox("setValue", stringIsEmpty(examPaper.paperCode) ? "" : examPaper.paperCode);
                }
            };

            function onTotalScoreChange() {
                loadData(examPaper.id);
            }

            function loadData(id) {
                VE.Mask("");
                var url = serviceUrl + "ExamPaper/Get?id=" + id;
                nv.get(url,
                    function (data) {
                        VE.UnMask();
                        if (data.success) {
                            setData(data.result);
                        } else {
                            $.messager.alert("提示", data.error.message, "info");
                        }
                    });
            }

            function setData(data) {
                examPaper = data;
                var funcText = examPaper.isCustomCode ? "uncheck" : "check";
                $("#isCustomCode").switchbutton(funcText);
                $("#paperCode").textbox("setValue", examPaper.paperCode);
                $("#paperName").textbox("setValue", examPaper.paperName);
                $("#remarks").textbox("setValue", examPaper.remarks);
                $("#isSingleAsMulti").combobox("setValue", examPaper.isSingleAsMulti);
                $("#paperHardGrade").combobox("setValue", examPaper.paperHardGrade);
                $("#folderUid").combotree("setValue", examPaper.folderUid);
                $("#outdatedDate").datebox("setValue", stringIsEmpty(examPaper.outdatedDate) ? "" : examPaper.outdatedDate);
                $("#totalScore").numberbox("setValue", examPaper.totalScore);
            }

            function getParam() {
                examPaper.isCustomCode = !($("#isCustomCode").switchbutton("options").checked);
                examPaper.paperCode = $("#paperCode").textbox("getValue");
                examPaper.paperName = $("#paperName").textbox("getValue");
                examPaper.remarks = $("#remarks").textbox("getValue");
                examPaper.isSingleAsMulti = $("#isSingleAsMulti").combobox("getValue");
                examPaper.paperHardGrade = $("#paperHardGrade").combobox("getValue");
                examPaper.folderUid = $("#folderUid").combotree("getValue");
                examPaper.outdatedDate = $("#outdatedDate").datebox("getValue");
                examPaper.totalScore = $("#totalScore").numberbox("getValue");
                return examPaper;
            }

            $(function () {
                $("#paperNode-container").hide();
                category.getCategory(function () {
                    if (!stringIsEmpty(id)) {
                        loadData(id);
                        paperNode.show(id);
                    }
                });
            });
        }
        return init;
    })(),
//试卷大题
    PaperNodeClass: (function () {

        function init() {
            var paperUid = "";
            var questionDic = {};
            var isListChanged = false;
            var tabIndex = getTabIndex();

            var handle = evtBus.addEvt("paper_node_flush", flush);
            var handle2 = evtBus.addEvt("tabs_tab_change", function(data) {
                if (!isListChanged || tabIndex !== data.index) {
                    return;
                }
                isListChanged = false;
                try {
                    $("#paperNode-dg").datagrid("resize").datagrid("fixRowHeight");
                } catch (e) {
                    alert(e);
                }
            });

            $(window).unload(function () {
                evtBus.removeEvt(handle);
                evtBus.removeEvt(handle2);
            });

            this.create = function() {
                var url = "/ExamPaper/ExamPaperNode/Edit?paperUid=" + paperUid;

                var title = "创建大题";
                parent.$("#tabs")
                    .tabs("add",
                    {
                        title: title,
                        content: iframeHtml.format({ title: title, url: url }),
                        closable: true,
                        icon: "icon-add"
                    });
            };

            this.edit = function(index) {
                $("#paperNode-dg").datagrid("selectRow", index);
                var row = $("#paperNode-dg").datagrid("getRows")[index];
                var url = "/ExamPaper/ExamPaperNode/Edit?paperUid={paperUid}&paperNodeUid={paperNodeUid}";
                var title = "编辑大题";
                parent.$("#tabs")
                    .tabs("add",
                    {
                        title: title,
                        content: iframeHtml.format({
                            title: title,
                            url: url.format({ paperUid: paperUid, paperNodeUid: row.id })
                        }),
                        closable: true,
                        icon: "icon-add"
                    });

            };

            this.del = function(index) {
                var checkedRows;
                if ($.isNumeric(index)) {
                    $("#paperNode-dg").datagrid("selectRow", index);
                    var row = $("#paperNode-dg").datagrid("getRows")[index];
                    checkedRows = [row];
                } else {
                    checkedRows = $("#paperNode-dg").datagrid("getChecked");
                    if (checkedRows.length === 0) {
                        $.messager.alert("提示", "请先选择要删除的项", "info");
                        return;
                    }
                }

                $.messager.confirm("删除确认",
                    "确定进行删除操作吗？",
                    function(b) {
                        if (!b) {
                            return;
                        }
                        if (!checkLogin()) {
                            evtBus.dispatchEvt("show_login");
                            return;
                        }
                        var idArray = [];
                        $.each(checkedRows,
                            function(k, v) {
                                idArray.push(v.id);
                            });

                        $("#paperNode-dg").datagrid("loading");
                        var url = serviceUrl + "ExamPaperNode/Delete?ids=" + idArray.join(",");
                        nv.get(url,
                            function(data) {
                                $("#paperNode-dg").datagrid("loaded");
                                if (data.success) {
                                    evtBus.dispatchEvt("total_score_change");
                                    $.messager.show({ title: "提示", msg: "删除成功" });
                                    flush();
                                } else {
                                    $.messager.alert("提示", data.error.message, "info");
                                }
                            });
                    });
            };

            this.show = function (id) {
                if (!stringIsEmpty(id)) {
                    paperUid = id;
                    getData(paperUid);
                }
                $("#paperNode-container").show();
                $("#paperNode-dg").datagrid("resize");
            };

            this.getPaperQuestion = function(paperNodeUid) {
                return questionDic[paperNodeUid];
            }

            function flush() {
                isListChanged = true;
                $.each(questionDic,
                    function (k, v) {
                        v.destory();
                    });
                questionDic = {};
                getData(paperUid);
            }

            function getData(paperUid) {
                $("#paperNode-dg").datagrid("loading");
                var url = serviceUrl + "ExamPaperNode/GetList?paperUid=" + paperUid;
                nv.get(url,
                    function (data) {
                        $("#paperNode-dg").datagrid("loaded");
                        if (data.success) {
                            $("#paperNode-dg").datagrid("loadData", data.result);
                        } else {
                            $.messager.alert("提示", data.error.message, "infol");
                        }
                    });
            }

            function initDataGrid() {
                questionDic = {};
                $("#paperNode-dg")
                    .datagrid({
                        view: detailview,
                        detailFormatter: function (index, row) {
                            var html = '<div style="padding:2px">' +
                                '<div id="toolbar-{id}">' +
                                '<a href="javascript:void(0)" class="easyui-linkbutton add" plain="true" iconCls="icon-add">添加</a>' +
                                '<a href="javascript:void(0)" class="easyui-linkbutton remove" plain="true" iconCls="icon-remove">删除</a>' +
                                '</div>' +
                                '<table id="{id}" class="ddv"></table>' +
                                '</div>';

                            return html.format({ id: row.id });
                        },
                        onExpandRow: function (index, row) {
                            if (!questionDic[row.id]) {
                                questionDic[row.id] = new paperEdit.PaperQuestionClass(row, index);
                            }
                        }
                    });
            }

            $(function () {
                initDataGrid();
            });
        }

        return init;
    })(),
//大题试题
    PaperQuestionClass: (function() {

        function init(paperNodeData, index) {
            var self = this;
            var isListChanged = false;
            var tabIndex = getTabIndex();
            var questionSelectedHandle = evtBus.addEvt("exam_question_selected", onExamQuestionSelected);
            var parentTabsChangeHandle = evtBus.addEvt("tabs_tab_change", function(data) {
                if (!isListChanged || data.index !== tabIndex) {
                    return;
                }
                isListChanged = false;
                $("#" + paperNodeData.id).datagrid("fixRowHeight");
            });
            //var nodeQuestionUpdateHandle = evtBus.addEvt("exam_paper_node_question_update", onExamPaperNodeQuestionUpdate);
            $(window).unload(function () {
                self.destory();
            });

            this.destory = function () {
                evtBus.removeEvt(questionSelectedHandle);
                //evtBus.removeEvt(nodeQuestionUpdateHandle);
                evtBus.removeEvt(parentTabsChangeHandle);
            }

            function onExamQuestionSelected(evt) {
                if (evt.handle === paperNodeData.id) {
                    var index = getTabIndex();
                    parent.$("#tabs").tabs("close", index);
                    parent.$("#tabs").tabs("select", tabIndex);
                    var ids = [];
                    isListChanged = true;
                    $.each(evt.data, function(k, v) {
                        ids.push(v.id);
                    });
                    create(ids);
                }
            }

            //function onExamPaperNodeQuestionUpdate() {
            //    getData(paperNodeData.id);
            //}

            function getData(paperNodeUid) {
                $("#" + paperNodeUid).datagrid("loading");
                var url = serviceUrl + "ExamPaperNodeQuestion/GetList?paperNodeUid=" + paperNodeUid;
                VE.Mask("");
                nv.get(url,
                    function (data) {
                        VE.UnMask();
                        $("#" + paperNodeUid).datagrid("loaded");
                        if (data.success) {
                            $("#" + paperNodeUid).datagrid("loadData", data.result);
                        } else {
                            $.messager.alert("提示", data.error.message, "info");
                        }
                    });
            }

            function showSelector() {
                if (!checkLogin()) {
                    evtBus.dispatchEvt("show_login");
                    return;
                }
                var url = "/Exam/Component/ExamQuestionSelector?id={id}&questionBaseTypeCode={questionBaseTypeCode}&exceptionType={exceptionType}";
                url = url.format({
                    id: paperNodeData.id,
                    questionBaseTypeCode: paperNodeData.questionBaseTypeCode,
                    exceptionType: "paperNode"
                });

                var title = "选择试题";
                parent.$("#tabs").tabs("add", {
                    title: title,
                    content: iframeHtml.format({ title: title, url: url }),
                    closable: true,
                    icon: "icon-ok"
                });
            }

            function create(ids) {
                if (!checkLogin()) {
                    evtBus.dispatchEvt("show_login");
                    return;
                }
                var url = serviceUrl + "ExamPaperNodeQuestion/Create";
                nv.post(url,
                    {
                        paperNodeUid: paperNodeData.id,
                        paperUid: paperNodeData.paperUid,
                        questionUidList: ids
                    },
                    function (data) {
                        if (data.success) {
                            getData(paperNodeData.id);
                            evtBus.dispatchEvt("total_score_change");
                            evtBus.dispatchEvt("paper_node_flush");
                            $.messager.show({ title: "提示", msg: "添加成功" });
                        } else {
                            $.messager.alert("提示", data.error.message, "info");
                        }
                    });
            }

            this.edit = function(rowIndex) {
                $("#" + paperNodeData.id).datagrid("selectRow", rowIndex);
                if (!checkLogin()) {
                    evtBus.dispatchEvt("show_login");
                    return;
                }
                var row = $("#" + paperNodeData.id).datagrid("getRows")[rowIndex];
                var url = "/ExamPaper/ExamPaperNodeQuestion/Edit?id=";
                var title = "编辑试题";
                parent.$("#tabs")
                    .tabs("add", {
                        title: title + row.questionCode,
                        content: iframeHtml.format({
                            title: title + row.questionCode,
                            url: url + row.id
                        }),
                        closable: true,
                        icon: "icon-edit"
                    });
            };

            this.del = function(rowIndex) {
                var checkedRows;
                if ($.isNumeric(rowIndex)) {
                    $("#" + paperNodeData.id).datagrid("selectRow", rowIndex);
                    var row = $("#" + paperNodeData.id).datagrid("getRows")[rowIndex];
                    checkedRows = [row];
                } else {
                    checkedRows = $("#" + paperNodeData.id).datagrid("getChecked");
                    if (checkedRows.length === 0) {
                        $.messager.alert("提示", "请先选中要删除的项", "info");
                        return;
                    }
                }
                $.messager.confirm("删除确认",
                    "确定要进行删除操作吗？",
                    function(b) {
                        if (!b) {
                            return;
                        }
                        if (!checkLogin()) {
                            evtBus.dispatchEvt("show_login");
                            return;
                        }
                        var idArray = [];
                        $.each(checkedRows,
                            function(k, v) {
                                idArray.push(v.id);
                            });
                        var url = serviceUrl + "ExamPaperNodeQuestion/Delete?nodeUid=" + paperNodeData.id + "&ids="
                            + idArray.join(",");
                        VE.Mask();
                        nv.get(url,
                            function(data) {
                                VE.UnMask();
                                if (data.success) {
                                    getData(paperNodeData.id);
                                    evtBus.dispatchEvt("total_score_change");
                                    evtBus.dispatchEvt("paper_node_flush");
                                    $.messager.show({ title: "提示", msg: "删除成功" });
                                } else {
                                    $.messager.alert("提示", data.error.message, "info");
                                }
                            });
                    });
            };

            function addEvent(paperNodeUid, b) {
                if (b) {
                    $("#toolbar-" + paperNodeUid + " a")
                        .on("click",
                            function () {
                                if ($(this).hasClass("add")) {
                                    showSelector();
                                } else if ($(this).hasClass("remove")) {
                                    self.del();
                                }
                            });
                } else {
                    $("#toolbar-" + paperNodeUid + " a").off("click");
                }
            }

            function initDataGrid(paperNodeUid, index) {
                $("#" + paperNodeUid)
                    .datagrid({
                        fitColumns: true,
                        singleSelect: false,
                        rownumbers: true,
                        loadMsg: "",
                        height: "auto",
                        toolbar: "#toolbar-" + paperNodeUid,
                        columns: [
                            [
                                { field: "ck", title: "", checkbox: true },
                                { field: "questionText", width:"50%", title: "题干", formatter: dgFormatter.questionText },
                                { field: "questionTypeName", width:"70px", title: "题型" },
                                { field: "paperQuestionExamTime", width:"90px", title: "每题时限（秒）" },
                                { field: "paperQuestionScore", width:"50px", title: "分值" },
                                { field: "opt", width:"116px", title: "操作", formatter: function(val, row, rowIndex) {
                                    var options = {
                                        option: [
                                            {
                                                title: "编辑",
                                                text: "编辑",
                                                icon: "icon-edit",
                                                onclick: "paperNode.getPaperQuestion('" + paperNodeUid + "').edit"
                                            }, {
                                                title: "删除",
                                                text: "删除",
                                                icon: "icon-remove",
                                                onclick: "paperNode.getPaperQuestion('" + paperNodeUid + "').del"
                                            }
                                        ]
                                    };

                                    return linkbtn(rowIndex, options);
                                } }
                            ]
                        ],
                        onResize: function () {
                            $("#paperNode-dg").datagrid("fixDetailRowHeight", index);
                        },
                        onLoadSuccess: function () {
                            $("#paperNode-dg").datagrid("fixDetailRowHeight", index);
                        }
                    });
                $("#paperNode-dg").datagrid("fixDetailRowHeight", index);
                addEvent(paperNodeUid, true);
            }

            initDataGrid(paperNodeData.id, index);
            getData(paperNodeData.id);
        }

        return init;
    })()
};

var PaperNodeEditClass = (function () {

    function init(paperUid, paperNodeUid) {
        var dataCache = {  paperUid: paperUid };
        this.save = function () {
            var validate = $("#paperNode-form").form("validate");
            if (!validate) {
                return;
            }
            if (!checkLogin()) {
                evtBus.dispatchEvt("show_login");
                return;
            }
            var questionScore = dataCache.questionScore;
            var param = getParam();
            var url = serviceUrl + "ExamPaperNode/";
            if (stringIsEmpty(param.id)) {
                url += "Create";
            } else {
                url += "Update";
            }

            VE.Mask();
            nv.post(url, param, function (data) {
                VE.UnMask();
                if (data.success) {
                    dataCache = param;
                    if (stringIsEmpty(param.id)) {
                        dataCache = data.result;
                        $("#questionTypeUid").combobox("disable");
                    } else {
                        if (param.questionScore !== questionScore && dataCache.questionNum > 0) {
                            evtBus.dispatchEvt("total_score_change");
                        }
                    }
                    evtBus.dispatchEvt("paper_node_flush");
                    $.messager.show({ title: "提示", msg: "保存成功" });
                } else {
                    $.messager.alert("提示", data.error.message, "info");
                }
            });
        };

        this.reset = function () {
            setValue(dataCache);
        };

        function loadData(paperNodeUid) {
            VE.Mask("");
            var url = serviceUrl + "ExamPaperNode/Get?id=" + paperNodeUid;
            nv.get(url,
                function (data) {
                    VE.UnMask();
                    if (data.success) {
                        dataCache = data.result;
                        setValue(dataCache);
                    } else {
                        $.messager.alert("提示", data.error.message, "info");
                    }
                });
        }

        function setValue(data) {
            $("#questionTypeUid").combobox("setValue", data.questionTypeUid);
            $("#paperNodeName").textbox("setValue", data.paperNodeName);
            $("#paperNodeDesc").textbox("setValue", data.paperNodeDesc);
            $("#questionScore").numberbox("setValue", data.questionScore);
            $("#planQuestionNum").numberbox("setValue", data.planQuestionNum);
            $("#listOrder").numberbox("setValue", data.listOrder);
        }

        function getParam() {
            var param = $.extend({}, dataCache);
            param.questionTypeUid = $("#questionTypeUid").combobox("getValue");
            param.paperNodeName = $("#paperNodeName").textbox("getValue");
            param.paperNodeDesc = $("#paperNodeDesc").textbox("getValue");
            param.questionScore = $("#questionScore").numberbox("getValue");
            param.planQuestionNum = $("#planQuestionNum").numberbox("getValue");
            param.listOrder = $("#listOrder").numberbox("getValue");
            return param;
        }


        $(function () {
            if (!stringIsEmpty(paperNodeUid)) {
                dataCache.id = paperNodeUid;
                loadData(paperNodeUid);
            }
        });
    }
    return init;
})();

var PaperNodeQuestionEditClass = (function () {
    
    function init(id) {
        var dataCache = {};
        this.save = function () {
            if (!checkLogin()) {
                evtBus.dispatchEvt("show_login");
                return;
            }
            var url = serviceUrl + "ExamPaperNodeQuestion/Update";
            VE.Mask();
            var param = getParam();
            nv.post(url, param, function (data) {
                VE.UnMask();
                if (data.success) {
                    if (data.paperQuestionScore != param.paperQuestionScore) {
                        evtBus.dispatchEvt("total_score_change");
                        evtBus.dispatchEvt("paper_node_flush");
                    }
                    //evtBus.dispatchEvt("exam_paper_node_question_update");
                    data = param;
                    $.messager.show({ title: "提示", msg: "保存成功" });
                } else {
                    $.messager.alert("提示", data.error.message, "info");
                }
            });
        }

        this.reset = function () {
            setData(dataCache);
        }

        function loadData(id) {
            var url = serviceUrl + "ExamPaperNodeQuestion/Get?id=" + id;
            VE.Mask();
            nv.get(url, function (data) {
                VE.UnMask();
                if (data.success) {
                    setData(data.result);
                } else {
                    $.messager.alert("提示", data.error.message, "info");
                }
            });
        }
        function getParam() {
            var param = $.extend({}, dataCache);
            param.paperQuestionScore = $("#paperQuestionScore").numberbox("getValue");
            param.paperQuestionExamTime = $("#paperQuestionExamTime").numberbox("getValue");
            param.listOrder = $("#listOrder").numberbox("getValue");
            return param;
        }

        function setData(data) {
            dataCache = data;
            $("#questionCode").text(data.questionCode);
            $("#questionTypeName").text(data.questionTypeName);
            $("#questionText").html(formatImg(data.questionText));
            $("#paperQuestionScore").numberbox("setValue", data.paperQuestionScore);
            $("#paperQuestionExamTime").numberbox("setValue", data.paperQuestionExamTime);
            $("#listOrder").numberbox("setValue", data.listOrder);
        }

        function formatImg(html) {
            var reg = /<IMG*.+?>/g;
            var srcReg = /src="\.\/\w.+?\.[A-Za-z]+"/;
            var match = reg.exec(html);
            if (match == null) {
                return html;
            }
            $.each(match, function (k, v) {
                var srcMatch = srcReg.exec(v);
                if (srcMatch.length == 0) {
                    return;
                }

                var src = srcMatch[0];
                var length = src.length - 8;//src="./"
                var url = src.substr(7, length);
                var newSrc = 'src="/fileroot/question/' + dataCache.questionUid + "/" + url + '"';
                html = html.replace(src, newSrc);
            });
            return html;
        }

        $(function () {
            loadData(id);
        });
    }
    return init;
})();
//获取tabs控件当前selected状态tab的index
function getTabIndex() {
    var tab = parent.$("#tabs").tabs("getSelected");
    return parent.$("#tabs").tabs("getTabIndex", tab);
}