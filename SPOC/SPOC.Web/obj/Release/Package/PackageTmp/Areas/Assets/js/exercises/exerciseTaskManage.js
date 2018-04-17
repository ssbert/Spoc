var ExerciseTaskManage = (function() {
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
            var url = "/Exercises/Manage/EditTask";
            tabHelper.openTab("新增练习任务", url, "icon-add", ["新增练习任务", "编辑练习任务"]);
        };

        this.edit = function(id) {
            if (!checkLogin()) {
                evtBus.dispatchEvt("show_login");
                return;
            }
            var url = "/Exercises/Manage/EditTask?id=" + id;
            tabHelper.openTab("编辑练习任务", url, "icon-edit", ["新增练习任务", "编辑练习任务"]);
        };

        this.del = function (index) {
            targetTask = $("#dg").datagrid("getRows")[index];
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
                    
                    var url = apiUrl + "ExerciseManage/Delete?id=" + targetTask.id;
                    $("#dg").datagrid("loading");
                    nv.get(url, function (data) {
                        $("#dg").datagrid("loaded");
                        if (data.success) {
                            $.messager.show({ title: "提示", msg: "删除成功！" });
                            loadData(queryParam);
                            if (targetTask.classes) {
                                $.messager.confirm("",
                                    "需要发送练习任务取消发布通知吗？",
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
            var url = apiUrl + "ExerciseManage/Publish";
            $("#classDialog").dialog("close");
            $("#dg").datagrid("loading");
            nv.post(url, param, function (data) {
                $("#dg").datagrid("loaded");
                if (data.success) {
                    $.messager.show({ title: "提示", msg: "发布成功！" });
                    $.messager.confirm("", "需要发送练习任务发布通知吗？", function (y) {
                        if (y) {
                            openNotification(param.idList);
                        }
                    });
                    loadData(queryParam);
                } else {
                    $.messager.alert("提示", data.error.message, "info");
                }
            });
        };

        this.showClassDialog = function (index) {
            if (!checkLogin()) {
                evtBus.dispatchEvt("show_login");
                return;
            }
            $("#classesForm").form("clear");
            targetTask = $("#dg").datagrid("getRows")[index];
            var url = apiUrl + "ExerciseManage/GetCandidateClasses?id=" + targetTask.id;
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
            var url = apiUrl + "ExerciseManage/GetPagination";
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
            var beginTime = stringIsEmpty(targetTask.beginTime) ? "不限" : targetTask.beginTime;
            var endTime = stringIsEmpty(targetTask.endTime) ? "不限" : targetTask.endTime;

            var content = "练习任务《{0}》已发布。".format(targetTask.title);
            content += "\n开始时间：{0}  结束时间：{1}".format(beginTime, endTime);

            notificationHelper.open("exercise", classIds, content);
        }

        function openDeleteNotification(classIds) {
            $("#classForm").form("clear");
            var content = "已取消练习任务《{0}》的发布。".format(targetTask.title);
            var notificationHelper = new NotificationHelper("tabs");
            notificationHelper.open("exercise", classIds, content);
        }
    }

    return init;
})();