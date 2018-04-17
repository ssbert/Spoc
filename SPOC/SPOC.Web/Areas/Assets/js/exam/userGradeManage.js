var UserGradeManage = (function () {
    function init(examTaskId) {
        var queryParam = { taskId: examTaskId, skip: 0, pageSize: 30 };
        var tabHelper = new TabHelper("tabs");
        this.query = function () {
            setParam();
            loadData(queryParam);
        };

        this.initPagination = function () {
            $("#dg")
                .datagrid({
                    onEndEdit: function (index, row, changes) {
                        if ($.isEmptyObject(changes)) {
                            return;
                        }
                        var param = [{
                            id: row.gradeId,
                            gradeScore: row.score,
                            isPass: row.isPass,
                            gradeStatusCode: row.gradeStatusCode
                        }];
                        $("#dg").datagrid("loading");
                        var url = apiUrl + "ExamGrade/Update";
                        nv.post(url, param, function (data) {
                            $("#dg").datagrid("loaded");
                            if (data.success) {
                                $.messager.show({ title: "提示", msg: "保存成功！" });
                                loadData(queryParam);
                            } else {
                                $.messager.alert("提示", data.error.message, "info");
                            }
                        });
                    },
                    onBeforeCellEdit: function () {
                        if (!checkLogin()) {
                            evtBus.dispatchEvt("show_login");
                            return false;
                        }
                        return true;
                    }
                })
                .datagrid("enableCellEditing")
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
        
        this.del = function (index) {
            if (!checkLogin()) {
                evtBus.dispatchEvt("show_login");
                return;
            }
            var ids = [];
            if (index) {
                var row = $("#dg").datagrid("getRows")[index];
                ids.push(row.gradeId);
            } else {
                var rows = $("#dg").datagrid("getChecked");
                if (rows.length === 0) {
                    return;
                }
                $.each(rows, function (k, v) {
                    ids.push(v.gradeId);
                });
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

                    var url = apiUrl + "ExamGrade/Delete";
                    $("#dg").datagrid("loading");
                    nv.post(url, { idList: ids }, function (data) {
                        $("#dg").datagrid("loaded");
                        if (data.success) {
                            $.messager.show({ title: "提示", msg: "删除成功！" });
                            loadData(queryParam);
                        } else {
                            $.messager.alert("提示", data.error.message, "info");
                        }
                    });

                });
        };

        this.showExamGrade = function (gradeId) {
            var url = "/Exam/Manage/UserExamPreview?examGradeUid=" + gradeId;
            tabHelper.openTab("考试结果", url, "icon-06");
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