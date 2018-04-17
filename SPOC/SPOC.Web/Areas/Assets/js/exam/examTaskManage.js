var ExamTaskManage = (function () {
    function init() {
        var queryParam = { skip: 0, pageSize: 30 };
        var tabHelper = new TabHelper("tabs");
        var targetTask;

        this.query = function() {
            setParam();
            loadData(queryParam);
        };

        this.add = function() {
            if (!checkLogin()) {
                evtBus.dispatchEvt("show_login");
                return;
            }
            var url = "/Exam/Manage/EditTask";
            tabHelper.openTab("新增考试任务", url, "icon-add", ["新增考试任务", "编辑考试任务"]);
        };

        this.edit = function(id) {
            if (!checkLogin()) {
                evtBus.dispatchEvt("show_login");
                return;
            }
            var url = "/Exam/Manage/EditTask?id=" + id;
            tabHelper.openTab("编辑考试任务", url, "icon-edit", ["新增考试任务", "编辑考试任务"]);
        };

        this.del = function (index) {
            //原本支持批量删除的，但新增通知功能无法适应，遂取消
            targetTask = $("#dg").datagrid("getRows")[index];
            var ids = [targetTask.id];

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
                    
                    var url = apiUrl + "ExamTask/Delete";
                    $("#dg").datagrid("loading");
                    nv.post(url, { idList: ids }, function (data) {
                        $("#dg").datagrid("loaded");
                        if (data.success) {
                            $.messager.show({ title: "提示", msg: "删除成功！" });
                            loadData(queryParam);
                            if (targetTask.classes) {
                                $.messager.confirm("",
                                    "需要发送考试任务取消发布通知吗？",
                                    function(y) {
                                        if (y) {
                                            var classIds = [];
                                            $.each(targetTask.classes, function(k) {
                                                classIds.push(k);
                                            });
                                            openDeleteNotification(classIds);
                                        }
                                    });
                            }
                        } else {
                            $.messager.alert("提示", data.error.message, "info");
                        }
                    });

                });
        };

        this.publish = function() {
            if (!$("#classesForm").form("validate")) {
                return;
            }
            var param = { taskId: targetTask.id, idList: $("#candidateClassIds").combobox("getValues") };
            var url = apiUrl + "ExamTask/Publish";
            $("#classDialog").dialog("close");
            $("#dg").datagrid("loading");
            nv.post(url, param, function (data) {
                $("#dg").datagrid("loaded");
                if (data.success) {
                    $.messager.show({ title: "提示", msg: "发布成功！" });
                    $.messager.confirm("", "需要发送考试任务发布通知吗？", function (y) {
                        if (y) {
                            openNotification(param.idList);
                        }
                    });
                    loadData(queryParam);
                } else {
                    $.messager.alert("提示", data.error.message, "info");
                }
                $("#classesForm").form("clear");
            });
        };

        this.showClassDialog = function (index) {
            if (!checkLogin()) {
                evtBus.dispatchEvt("show_login");
                return;
            }
            $("#classesForm").form("clear");
            targetTask = $("#dg").datagrid("getRows")[index];
            var url = apiUrl + "ExamTask/GetCandidateClasses?id=" + targetTask.id;
            nv.get(url, function (data) {
                if (data.success) {
                    $("#classDialog").dialog("open");
                    $("#candidateClassIds").combobox("loadData", data.result);

                } else {
                    $.messager.alert("提示", data.error.message, "info");
                }
            });
        };

        this.initPagination = function() {
            $("#dg").datagrid("getPager")
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

        function setParam() {
            queryParam.code = $("#code").textbox("getValue").trim();
            queryParam.title = $("#title").textbox("getValue").trim();
            queryParam.userFullName = $("#userFullName").textbox("getValue").trim();
            queryParam.userLoginName = $("#userLoginName").textbox("getValue").trim();
            queryParam.classIds = $("#classIds").combotree("getValues");
            queryParam.createBeginTime = $("#createBeginTime").datetimebox("getValue").trim();
            queryParam.createEndTime = $("#createEndTime").datetimebox("getValue").trim();
            queryParam.beginTime = $("#beginTime").datetimebox("getValue").trim();
            queryParam.endTime = $("#endTime").datetimebox("getValue").trim();
        }

        function loadData(param) {
            $("#dg").datagrid("loading");
            var url = apiUrl + "ExamTask/GetPagination";
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

        function openNotification(classIds) {
            var notificationHelper = new NotificationHelper("tabs");
            var url = apiUrl + "ExamTask/Get?id=" + targetTask.id;
            $("#dg").datagrid("loading");
            nv.get(url,
                function(data) {
                    $("#dg").datagrid("loaded");
                    if (data.success) {
                        var content = "考试任务《{0}》已发布。".format(targetTask.title);
                        
                        $.each(data.result.exams, function (k, v) {
                            var exam = {
                                examName: v.examName,
                                beginTime: !stringIsEmpty(v.beginTime) ? v.beginTime : "不限",
                                endTime: !stringIsEmpty(v.endTime) ? v.endTime : "不限",
                                examTime: v.examTime === 0 ? "不限" : (v.examTime / 60) + "分钟",
                                maxExamNum: v.maxExamNum ? v.maxExamNum + "次" : "不限"
                            }
                            content += "\n考试：{examName}\n" +
                                "开始时间：{beginTime}  考试结束时间：{endTime} \n" +
                                "时长：{examTime}\n" +
                                "次数：{maxExamNum}\n";
                            content = content.format(exam);
                            if (v.passGradeType === "passGradeRate") {
                                var passGradeRate = v.passGradeRate && v.passGradeRate !== 0 ? v.passGradeRate + "%" : "不限";
                                content += "通过率：" + passGradeRate;
                            } else if (v.passGradeType === "passGradeScore") {
                                var passGradeScore = v.passGradeScore && v.passGradeScore !== 0 ? v.passGradeScore + "分" : "不限";
                                content += "通过分数：" + passGradeScore;
                            }
                        });
                        
                        notificationHelper.open("exam", classIds, content);
                    } else {
                        $.messager.alert("提示", data.error.message, "info");
                    }
                });
        }

        function openDeleteNotification(classIds) {
            var content = "已取消考试任务《{0}》的发布。".format(targetTask.title);
            var notificationHelper = new NotificationHelper("tabs");
            notificationHelper.open("exam", classIds, content);
        }
    }

    return init;
})();