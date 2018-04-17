var serviceUrl = "/api/services/app/";
var nv = nv ? nv : {};
nv.components = {};
nv.componentData = {};

//试题选择器
//使用ExamQuestionSelector组件时，组件返回数据是基于事件的
//evtBus.dispatchEvt("exam_question_selected", data)
nv.components.ExamQuestionSelectorClass = (function () {

    function init(param) {
       
        var self = this;
        var pageNumberCache = 1;
        var selectedData = [];
        var selectedDataDic = {};
        var existingQuestionIds = [];
        var selectedQuestionIds = [];      
        param = param ? param : {};
        param.skip = 0;
        param.pageSize = 30;
        param.sort = "";
        param.order = "";

        self.handle = param.handle;
        var category = new nv.category.CombotreeDataClass("folderUid", "question_bank");

        this.addToSelected = function () {
            var checkedRows = $("#question-dg").datagrid("getChecked");
            if (checkedRows.length === 0) {
                $.messager.alert("提示", "请先进行选择", "info");
                return;
            }

            $.each(checkedRows,
                function (k, v) {
                    if (!selectedDataDic[v.id]) {
                        var question = $.extend({}, v);
                        question.ck = false;
                        selectedData.push(question);
                        selectedDataDic[question.id] = question;
                        selectedQuestionIds.push(question.id);
                    }
                });
            param.exceptionIdList = existingQuestionIds.concat(selectedQuestionIds);
            getData(param);
            $("#selected-dg").datagrid("loadData", selectedData);
        };

        this.remove = function () {
            var checkedRows = $("#selected-dg").datagrid("getChecked");
            if (checkedRows.length === 0) {
                $.messager.alert("提示", "请先进行选择", "info");
                return;
            }
            $.messager.confirm("删除确认",
                "确定进行删除操作吗？",
                function (b) {
                    if (!b) {
                        return;
                    }
                    $.each(checkedRows,
                        function (k, v) {
                            var index = selectedData.indexOf(v);
                            if (index >= 0) {
                                selectedData.splice(index, 1);
                                delete selectedDataDic[v.id];
                            }
                            index = selectedQuestionIds.indexOf(v.id);
                            if (index >= 0) {
                                selectedQuestionIds.splice(index, 1);
                            }
                        });

                    $("#selected-dg").datagrid("loadData", selectedData);
                    param.exceptionIdList = existingQuestionIds.concat(selectedQuestionIds);
                    getData(param);
                });
        };

        this.selected = function () {
            if (selectedData.length === 0) {
                $.messager.alert("提示", "没选择的试题", "info");
                return;
            }

            evtBus.dispatchEvt("exam_question_selected",
            {
                data: selectedData,
                handle: self.handle
            });
        };

        this.query = function () {
            getData(getParam());
        };

        this.queryReset = function () {
            $('#query-form').form('reset');
            $("#questionBaseTypeCode").combobox("setValue", param.questionBaseTypeCode);
        };

        function initPagination() {
            $("#question-dg")
                .datagrid({
                    onSortColumn: function (sort, order) {
                        param.sort = sort;
                        param.order = order;
                        self.query();
                    }
                })
                .datagrid("getPager").pagination({
                onSelectPage: function (pageNumber, pageSize) {
                    pageNumberCache = pageNumber;
                    param.skip = (pageNumber - 1) * pageSize;
                    param.pageSize = pageSize;
                    getData(param);
                },
                onChangePageSize: function (pageSize) {
                    param.pageSize = pageSize;
                    getData(param);
                }
            });
        }

        function getData(param) {
            var url = serviceUrl + "QuestionBank/GetPagination";
            $("#question-dg").datagrid("loading");
            nv.post(url, param, function (data) {
                $("#question-dg").datagrid("loaded");
                if (data.success) {
                    $("#question-dg")
                        .datagrid("loadData", data.result.rows)
                        .datagrid("getPager")
                        .pagination({
                            pageNumber: pageNumberCache,
                            total: data.result.total
                        });
                } else {
                    $.messager.alert("提示", data.error.message, "info");
                }

            });
        }

        function getParam() {
            param.questionText = $("#questionText").textbox("getValue");
            param.questionCode = $("#questionCode").textbox("getValue");
            param.questionBaseTypeCode = $("#questionBaseTypeCode").combobox("getValue");
            param.folderUidList = $("#folderUid").combotree("getValues");
            param.questionStatusCode = $("#questionStatusCode").combobox("getValue");
            param.hardGrade = $("#hardGrade").combobox("getValue");
            param.hasChild = "N";
            param.hasAnalysis = $("#hasAnalysis").combobox("getValue");
            return param;
        }

        $(function () {
            if (!stringIsEmpty(param.questionBaseTypeCode)) {
                $("#questionBaseTypeCode").combobox("setValue", param.questionBaseTypeCode).combobox("disable");
            }
            initPagination();

            var waitCallbackCount = 1;
            var initData = function () {
                waitCallbackCount--;
                if (waitCallbackCount === 0) {
                    getData(getParam());
                }
            };
            if (param.exception) {
                waitCallbackCount++;
                param.exception.getData(function (idList) {
                    existingQuestionIds = idList;
                    param.exceptionIdList = [].concat(existingQuestionIds);
                    initData();
                });
            }
            category.getCategory(initData);
            $("#selected-dg").datagrid({ data: selectedData }).datagrid("clientPaging");
        });
    }

    return init;
})();

nv.componentData.PaperNodeQuestionUidClass = (function () {
    function init(paperNodeUid) {
        this.getData = function (callback) {
            var url = serviceUrl + "ExamPaperNodeQuestion/GetIdList?paperNodeUid=" + paperNodeUid;
            VE.Mask("");
            nv.get(url, function (data) {
                VE.UnMask();
                if (data.success) {
                    if (callback) {
                        callback(data.result);
                    }
                } else {
                    $.messager.alert("提示", data.error.message, "info");
                }
            });
        }
    }

    return init;
})();

nv.componentData.ExercisesBankQuestionidClass = (function() {
    function init(exercisesBankId) {
        this.getData = function(callback) {
            var url = serviceUrl + "ExercisesBankQuestion/GetIdList?exercisesBankId=" + exercisesBankId;
            VE.Mask("");
            nv.get(url, function(data) {
                VE.UnMask();
                if (data.success) {
                    if (callback) {
                        callback(data.result);
                    }
                } else {
                    $.messager.alert("提示", data.error.message, "info");
                }
            });
        }
    }

    return init;
})();

//试卷选择器
nv.components.ExamPaperSelectorClass = (function () {
    
    function init() {
        var self = this;
        var param = {
            paperCode: "",
            paperName: "",
            paperTypeCode: "",
            departmentUidList: [],
            subjectUidList: [],
            folderUidList: [],
            checkOutDate: true,
            pageNumber: 1,
            skip: 0,
            pageSize: 30
        };
        var category = new nv.category.CombotreeDataClass("folderUid", "exam_paper");
        this.query = function () {
            var param = getQueryParam();
            param.skip = 0;
            getData(param);
        };
        this.optFormatter = function (val, row, index) {
            var options = {
                option: [{
                    title: "选择",
                    text: "选择",
                    icon: "icon-ok",
                    onclick: "selector.onSelectRow"
                }]
            };
            return linkbtn(index, options);
        };

        this.paperTypeCodeFormatter = function (val) {
            if (val === "fix") {
                return "固定试卷";
            }

            if (val === "random") {
                return "随机试卷";
            }

            return val;
        };

        this.onSelectRow = function (index) {
            var row = $("#dg").datagrid("getRows")[index];
            evtBus.dispatchEvt("exam_paper_selected", row);
        }

        function initPagination() {
            $("#dg")
                .datagrid("getPager")
                .pagination({
                    onSelectPage: function (pageNumber, pageSize) {
                        param.pageNumber = pageNumber;
                        param.skip = (pageNumber - 1) * pageSize;
                        if (param.skip < 0) {
                            param = 0;
                        }
                        param.pageSize = pageSize;
                        getData(param);
                    },
                    onChangePageSize: function (pageSize) {
                        param.pageSize = pageSize;
                        getData(param);
                    }
                });
        }

        function getData(param) {
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
            param.paperCode = $("#paperCode").textbox("getValue").trim();
            param.paperName = $("#paperName").textbox("getValue").trim();
            param.paperTypeCode = $("#paperTypeCode").combobox("getValue");
            param.folderUidList = $("#folderUid").combotree("getValues");
            return param;
        }

        $(function () {
            category.getCategory();
            initPagination();
            self.query();
        });
    }

    return init;
})();


//单个试题选择器
nv.components.QuestionSingleSelectorClass = (function() {
    function init(type, handle, tabContainerId) {
        var attr = { questionBaseTypeCode: type, skip: 0, pageSize: 30 };
        var tabHelper = new TabHelper(tabContainerId);
        var tabIndex = tabHelper.getTabIndex();

        this.query = function () {
            getData(getParam());
        };

        this.choose = function (index) {
            var row = $("#question-dg").datagrid("getRows")[index];
            evtBus.dispatchEvt("question_single_selected",
                {
                    data: row,
                    handle: handle
                });
            tabHelper.closeTab(tabIndex);
        };

        this.initPagination = function () {
            $("#question-dg")
                .datagrid({
                    onSortColumn: function (sort, order) {
                        attr.sort = sort;
                        attr.order = order;
                        self.query();
                    }
                })
                .datagrid("getPager").pagination({
                    onSelectPage: function (pageNumber, pageSize) {
                        attr.pageNumber = pageNumber;
                        attr.skip = (pageNumber - 1) * pageSize;
                        attr.pageSize = pageSize;
                        getData(attr);
                    },
                    onChangePageSize: function (pageSize) {
                        attr.pageSize = pageSize;
                        getData(attr);
                    }
                });
        }

        function getData(param) {
            var url = serviceUrl + "QuestionBank/GetPagination";
            $("#question-dg").datagrid("loading");
            nv.post(url, param, function (data) {
                $("#question-dg").datagrid("loaded");
                if (data.success) {
                    $("#question-dg")
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

        function getParam() {
            var param = $.extend({}, attr);
            param.questionText = $("#questionText").textbox("getValue");
            param.questionCode = $("#questionCode").textbox("getValue");
            param.folderUidList = $("#folderUid").combotree("getValues");
            param.questionStatusCode = $("#questionStatusCode").combobox("getValue");
            param.hardGrade = $("#hardGrade").combobox("getValue");
            param.hasChild = "N";
            param.hasAnalysis = $("#hasAnalysis").combobox("getValue");
            return param;
        }
    }

    return init;
})();