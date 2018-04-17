var JudgeManage = (function () {
    function init(examTaskId) {
        var queryParam = { taskId: examTaskId, skip: 0, pageSize: 30 };
        var tabHelper = new TabHelper("tabs");

        this.query = function () {
            setParam();
            loadData(queryParam);
        };

        this.initPagination = function () {

            $("#dg")
                .datagrid("getPager")
                .pagination({
                    onSelectPage: function (pageNumber, pageSize) {
                        queryParam.pageNumber = pageNumber;
                        queryParam.skip = (pageNumber - 1) * pageSize;
                        if (queryParam.skip < 0) {
                            queryParam.skip = 0;
                        }
                        queryParam.pageSize = pageSize;
                        loadData(queryParam);
                    },
                    onChangePageSize: function (pageSize) {
                        queryParam.pageSize = pageSize;
                        loadData(queryParam);
                    }
                });
        };
        
        this.judge = function (gradeId) {
            if (!checkLogin()) {
                evtBus.dispatchEvt("show_login");
                return;
            }
            var url = "/Exam/Manage/JudgeExam?examGradeUid=" + gradeId;
            tabHelper.openTab("手工评卷", url, "icon-edit");
        };

        function setParam() {
            queryParam.userFullName = $("#userFullName").textbox("getValue").trim();
            queryParam.userLoginName = $("#userLoginName").textbox("getValue").trim();
            queryParam.classIds = $("#classIds").combotree("getValues");
            queryParam.beginTime = $("#beginTime").datetimebox("getValue").trim();
            queryParam.endTime = $("#endTime").datetimebox("getValue").trim();
            queryParam.examTypeCode = $("#examTypeCode").combobox("getValue");
            queryParam.gradeStatusCode = $("#gradeStatusCode").combobox("getValue");
            queryParam.minScore = $("#minScore").numberbox("getValue");
            queryParam.maxScore = $("#maxScore").numberbox("getValue");
        }

        function loadData(param) {
            $("#dg").datagrid("loading");
            var url = apiUrl + "ExamGrade/GetPagination";
            nv.post(url, param, function (data) {
                $("#dg").datagrid("loaded");
                if (data.success) {
                    editList = [];
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
    }

    return init;
})();